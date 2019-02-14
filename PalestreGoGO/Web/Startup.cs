using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.HttpOverrides;
using Web.Services;
using Web.Configuration;
using Microsoft.AspNetCore.Mvc.Razor;
using Web.Utils;
using Microsoft.AspNetCore.Authentication;
using System.Globalization;
using Microsoft.AspNetCore.Localization;
using FluentValidation.AspNetCore;
using Web.Authorization;
using Microsoft.AspNetCore.Authorization;
using Web.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using System.Threading;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Tokens;
using Web.Proxies;


//using 
namespace Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }


        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {

            services.AddOptions();
            services.Configure<AppConfig>(_configuration.GetSection("AppConfig"));
            services.Configure<GraphAPIOptions>(_configuration.GetSection("Authentication:GraphAPI"));

            services.Configure<B2CAuthenticationOptions>(_configuration.GetSection("Authentication:AzureAdB2C"));
            services.Configure<B2CPolicies>(_configuration.GetSection("Authentication:AzureAdB2C:Policies"));

            ConfigureProxies(services);
            // Add application services.
            services.AddTransient<ClienteResolverServices, ClienteResolverServices>();

            //Aggiungiamo la cache in memory
            services.AddDistributedMemoryCache();

            //TODO: CONFIGURARE DPAPI PER USARE UN CERTIFICATO SU AZURE (STORAGE O VAULT)
            // vedi: https://docs.microsoft.com/en-us/aspnet/core/security/data-protection/implementation/key-storage-providers#azure-and-redis
            //services.AddDataProtection()
            //    .PersistKeysToAzureBlobStorage(new Uri("<blob URI including SAS token>"));
              
            services.AddMvc(options =>{options.Filters.Add(typeof(ReauthenticationRequiredFilter));})
                .SetCompatibilityVersion(Microsoft.AspNetCore.Mvc.CompatibilityVersion.Version_2_2)
                .AddFluentValidation(fv =>
                {
                    fv.RegisterValidatorsFromAssemblyContaining(this.GetType());
                    fv.RunDefaultMvcValidationAfterFluentValidationExecutes = true; //Per ora lasciamo abilitata la validazione di default (DataAnnotations) fino alla migrazione completa
                });

            services.AddScoped<ReauthenticationRequiredFilter>();
            services.AddAuthorization(options =>
            {
                options.AddPolicy("CanEditStruttura", policy => policy.Requirements.Add(new CadEditStrutturaRequirement()));                
            });
            services.AddSingleton<IAuthorizationHandler, CanEditStrutturaHandler>();

            services.Configure<RazorViewEngineOptions>(options =>
            {
                options.ViewLocationExpanders.Add(new SharedViewsLocationExpander());
            });

            services.AddSession(options =>
            {
                options.IdleTimeout = TimeSpan.FromHours(1);
                options.Cookie.HttpOnly = true;
            });

            ConfigureAuthentication(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            //Usiamo la cultura Americana per gestire il parsing dei decimali
            var cultureInfo = new CultureInfo("en-US");
            cultureInfo.NumberFormat.CurrencySymbol = "€";

            CultureInfo.DefaultThreadCurrentCulture = cultureInfo;
            CultureInfo.DefaultThreadCurrentUICulture = cultureInfo;

            app.UseRequestLocalization(new RequestLocalizationOptions
            {
                DefaultRequestCulture = new RequestCulture("it-IT"),
                SupportedCultures = new List<CultureInfo> { new CultureInfo("en-US") },
                SupportedUICultures = new List<CultureInfo> { new CultureInfo("en-US") }
            });

            //Necessario per usare Nginx come reverse proxy (altrimenti non funziona l'authentication)
            app.UseForwardedHeaders(new ForwardedHeadersOptions
            {
                ForwardedHeaders = ForwardedHeaders.XForwardedFor | ForwardedHeaders.XForwardedProto
            });


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseBrowserLink();
            }
            else
            {
                app.UseExceptionHandler("/Home/Error");
            }

            app.UseHttpsRedirection();
            app.UseSession();
            app.UseAuthentication();
            app.UseStaticFiles();
            app.UseMvc(routes =>
            {
                //routes.MapRoute(
                //    name: "logout",
                //    template: "logout",
                //    defaults: new { controller = "Account", action = "Logout" });
                routes.MapRoute(
                    name: "users",
                    template: "users/myprofile",
                    defaults: new { controller = "Users", action = "Index" }
                    );
                routes.MapRoute(
                    name: "clienti",
                    template: "{cliente:required}/{controller=Clienti}/{action=Index}/{id?}"
                    );
                routes.MapRoute(
                    name: "default",
                    template: "{controller=Home}/{action=Index}/{id?}");

            });
        }

        private void ConfigureProxies(IServiceCollection services)
        {
            services.AddTransient<TipologicheProxy, TipologicheProxy>();
            services.AddTransient<ClienteProxy, ClienteProxy>();
            services.AddTransient<NotificheProxy, NotificheProxy>();
            services.AddTransient<SchedulesProxy, SchedulesProxy>();
            services.AddTransient<UtentiProxy, UtentiProxy>();
        }

        private static void ConfigureAuthentication(IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            var authOptions = serviceProvider.GetService<IOptions<B2CAuthenticationOptions>>();
            var b2cPolicies = serviceProvider.GetService<IOptions<B2CPolicies>>();

            var distributedCache = serviceProvider.GetService<IDistributedCache>();
            services.AddSingleton(distributedCache);

            services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Constants.OpenIdConnectAuthenticationScheme;
            })
            .AddCookie()
            .AddOpenIdConnect(Constants.OpenIdConnectAuthenticationScheme, options =>
            {
                options.Authority = authOptions.Value.Authority;
                options.ClientId = authOptions.Value.ClientId;
                options.ClientSecret = authOptions.Value.ClientSecret;
                options.SignedOutRedirectUri = authOptions.Value.PostLogoutRedirectUri;

                options.ConfigurationManager = new PolicyConfigurationManager(authOptions.Value.Authority,
                                               new[] { b2cPolicies.Value.SignInOrSignUpPolicy, b2cPolicies.Value.EditProfilePolicy, b2cPolicies.Value.ResetPasswordPolicy });

                options.Events = CreateOpenIdConnectEventHandlers(authOptions.Value, b2cPolicies.Value, distributedCache);

                options.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    NameClaimType = "name"
                };

                // it will fall back on using DefaultSignInScheme if not set
                //options.SignInScheme = CookieAuthenticationDefaults.AuthenticationScheme;

                // we have to set these scope that will be used in /authorize request
                // (otherwise the /token request will not return access and refresh tokens)
                options.Scope.Add("offline_access");
                options.Scope.Add(authOptions.Value.ApiScopes);

                // this can be used if the middleware redeems the authorization code
                //options.SaveTokens = true;
            });
        }

        private static OpenIdConnectEvents CreateOpenIdConnectEventHandlers(B2CAuthenticationOptions authOptions, B2CPolicies policies, IDistributedCache distributedCache)
        {
            return new OpenIdConnectEvents
            {
                OnRedirectToIdentityProvider = context => SetIssuerAddressAsync(context, policies.SignInOrSignUpPolicy),
                OnRedirectToIdentityProviderForSignOut = context => SetIssuerAddressForSignOutAsync(context, policies.SignInOrSignUpPolicy),
                OnAuthorizationCodeReceived = async context =>
                {
                    try
                    {
                        var principal = context.Principal;

                        var userTokenCache = new DistributedTokenCache(distributedCache, principal.FindFirst(Constants.ObjectIdClaimType).Value).GetMSALCache();
                        var client = new ConfidentialClientApplication(authOptions.ClientId,
                            authOptions.GetAuthority(principal.FindFirst(Constants.AcrClaimType).Value),
                            "https://app", // it's not really needed
                            new ClientCredential(authOptions.ClientSecret),
                            userTokenCache,
                            null);

                        var result = await client.AcquireTokenByAuthorizationCodeAsync(context.TokenEndpointRequest.Code,
                            new[] { authOptions.ApiScopes });

                        context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                        /*20190214#GT#Gestione nuovi utenti*/
                    }
                    catch (Exception ex)
                    {
                        context.Fail(ex);
                    }
                },
                OnAuthenticationFailed = context =>
                {
                    context.Fail(context.Exception);
                    return Task.FromResult(0);
                },
                OnMessageReceived = context =>
                {
                    if (!string.IsNullOrEmpty(context.ProtocolMessage.Error) &&
                        !string.IsNullOrEmpty(context.ProtocolMessage.ErrorDescription))
                    {
                        if (context.ProtocolMessage.ErrorDescription.StartsWith("AADB2C90091")) // cancel profile editing
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/");
                        }
                        else if (context.ProtocolMessage.ErrorDescription.StartsWith("AADB2C90118")) // forgot password
                        {
                            context.HandleResponse();
                            context.Response.Redirect("/Account/ResetPassword");
                        }
                    }

                    return Task.FromResult(0);
                }
            };
        }

        private static async Task SetIssuerAddressAsync(RedirectContext context, string defaultPolicy)
        {
            var configuration = await GetOpenIdConnectConfigurationAsync(context, defaultPolicy);
            context.ProtocolMessage.IssuerAddress = configuration.AuthorizationEndpoint;
        }

        private static async Task SetIssuerAddressForSignOutAsync(RedirectContext context, string defaultPolicy)
        {
            var configuration = await GetOpenIdConnectConfigurationAsync(context, defaultPolicy);
            context.ProtocolMessage.IssuerAddress = configuration.EndSessionEndpoint;
        }

        private static Task<OpenIdConnectConfiguration> GetOpenIdConnectConfigurationAsync(RedirectContext context, string defaultPolicy)
        {
            var manager = (PolicyConfigurationManager)context.Options.ConfigurationManager;
            var policy = context.Properties.Items.ContainsKey(Constants.B2CPolicy) ? context.Properties.Items[Constants.B2CPolicy] : defaultPolicy;

            return manager.GetConfigurationByPolicyAsync(CancellationToken.None, policy);
        }
    }
}
