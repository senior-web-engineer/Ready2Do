using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using PalestreGoGo.WebAPIModel;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using Web.Proxies;
using Web.Utils;

namespace Web.Authentication
{
    public static class B2CAuthenticationExtensions
    {
        private static B2CAuthenticationOptions B2CAuthenticationOptions;
        private static IDistributedCache DistributedCache;
        private static UserIdTokensMonitorService IdTokenRefreshMonitor;
        private static UtentiProxy UserProxy;

        public static void AddAzureAdB2Authentication(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            DistributedCache = serviceProvider.GetService<IDistributedCache>();
            IdTokenRefreshMonitor = serviceProvider.GetService<UserIdTokensMonitorService>();
            UserProxy = serviceProvider.GetService<UtentiProxy>();
            services.AddSingleton(DistributedCache);

            B2CAuthenticationOptions = serviceProvider.GetService<IOptions<B2CAuthenticationOptions>>().Value;
            //var b2cPolicies = serviceProvider.GetService<IOptions<B2CPolicies>>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Constants.OpenIdConnectAuthenticationScheme;
            })
            .AddCookie(options =>
            {
                options.Events = new CookieAuthenticationEvents()
                {
                    OnValidatePrincipal = OnValidatePrincipalHandler
                };
            })
            .AddOpenIdConnect(Constants.OpenIdConnectAuthenticationScheme, options =>
            {
                options.Authority = B2CAuthenticationOptions.Authority;
                options.ClientId = B2CAuthenticationOptions.ClientId;
                options.ClientSecret = B2CAuthenticationOptions.ClientSecret;
                options.SignedOutRedirectUri = B2CAuthenticationOptions.PostLogoutRedirectUri;

                options.ConfigurationManager = new PolicyConfigurationManager(B2CAuthenticationOptions.Authority,
                                               new[] { B2CAuthenticationOptions.Policies.SignInOrSignUpPolicy,
                                                       B2CAuthenticationOptions.Policies.EditProfilePolicy,
                                                       B2CAuthenticationOptions.Policies.ResetPasswordPolicy });

                options.Events = new OpenIdConnectEvents()
                {
                    OnAuthenticationFailed = OnAuthenticationFailedHandler,
                    OnAuthorizationCodeReceived = OnAuthorizationCodeReceivedHandler,
                    OnMessageReceived = OnMessageReceivedHandler,
                    OnRedirectToIdentityProvider = OnRedirectToIdentityProviderHandler,
                    OnRedirectToIdentityProviderForSignOut = OnRedirectToIdentityProviderForSignOutHandler,
                    OnRemoteFailure = OnRemoteFailureHandler,
                    OnRemoteSignOut = OnRemoteSignOutHandler,
                    OnSignedOutCallbackRedirect = OnSignedOutCallbackRedirectHandler,
                    OnTicketReceived = OnTicketReceivedHandler,
                    OnTokenResponseReceived = OnTokenResponseReceivedHandler,
                    OnTokenValidated = OnTokenValidatedHandler,
                    OnUserInformationReceived = OnUserInformationReceivedHandler
                };
                //CreateOpenIdConnectEventHandlers(authOptions.Value, b2cPolicies.Value, distributedCache);

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
                options.Scope.Add(B2CAuthenticationOptions.ApiScopes);

                // this can be used if the middleware redeems the authorization code
                //options.SaveTokens = true;
            });

        }

        /// <summary>
        /// Personalizzazione della validazione del Cookie.
        /// E' necessario invalidare e ricrere il cookie di autenticazione nei casi in cui cambi lo stato dell'utente dopo la creazione del cookie.
        /// I casi da gestire sono:
        /// - registrazione nuova struttura (il claim delle strutture owned nel cookie non è più allineato)
        /// - conferma email (è necessario aggiungere il claim di mail validata)
        /// Per gestire questa modifica utiliziamo un altro cookie valorizzato al momento della modifica che andiamo a cercare ad ogni richiesta.
        /// Se il cookie delle modifiche è presente incorporiamo la modifica nel cookie di autenticazione e lo eliminiamo
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        private static async Task OnValidatePrincipalHandler(CookieValidatePrincipalContext context)
        {
            if (!context.Principal.Identity.IsAuthenticated) { return; }
            var changes = context.Request.Cookies[Constants.COOKIE_USERCHANGES_KEY];
            if (string.IsNullOrWhiteSpace(changes)) { return; }
            string userId = context.Principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ObjectIdClaimType))?.Value;
            string authnclassreference = context.Principal.Claims.FirstOrDefault(c => c.Type.Equals(Constants.AcrClaimType))?.Value;
            AzureUser azUser = default(AzureUser);
            try
            {
                azUser = await UserProxy.GetMyIdPProfileAsync(userId, authnclassreference);
            }
            catch (ReauthenticationRequiredException exc)
            {
                Log.Warning(exc, "Necessaria riautenticazione per l'utente {userId}", userId);
                context.RejectPrincipal();
                context.ShouldRenew = true;
                return;
            }
            ClaimsIdentity identity = new ClaimsIdentity(context.Principal.Identity);
            //Gestione claim Strutture gestite
            var oldClaimStruttureOwned = identity.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimStructureOwned));
            if (oldClaimStruttureOwned != null)
            {
                identity.RemoveClaim(oldClaimStruttureOwned);
            }
            if (!string.IsNullOrEmpty(azUser.StruttureOwned))
            {
                identity.AddClaim(new Claim(Constants.ClaimStructureOwned, azUser.StruttureOwned));
            }
            //Gestione claim MailConfirmed
            var oldClaimMailConfirmed = identity.Claims.FirstOrDefault(c => c.Type.Equals(Constants.ClaimEmailConfirmedOn));
            if (oldClaimMailConfirmed != null) { identity.RemoveClaim(oldClaimMailConfirmed); }
            if (!string.IsNullOrWhiteSpace(azUser.EmailConfirmedOn)) { identity.AddClaim(new Claim(Constants.ClaimEmailConfirmedOn, azUser.EmailConfirmedOn)); }

            //Rimpiaziamo il principal e marchiamo il Cookie da ricreare
            ClaimsPrincipal newPrincipal = new ClaimsPrincipal(identity);
            context.ReplacePrincipal(newPrincipal);
            context.ShouldRenew = true;
            //IdTokenRefreshMonitor.RemoveUserToRefresh(newPrincipal.UserId());
            //Rimuoviamo il cookie
            context.Response.Cookies.Delete(Constants.COOKIE_USERCHANGES_KEY);
        }

        #region HANDLERS EVENTI OPENID
        private static Task OnAuthenticationFailedHandler(AuthenticationFailedContext context)
        {
            context.Fail(context.Exception);
            return Task.CompletedTask;
        }
        private static async Task OnAuthorizationCodeReceivedHandler(AuthorizationCodeReceivedContext context)
        {
            try
            {
                var principal = context.Principal;
                var userTokenCache = new DistributedTokenCache(DistributedCache, principal.FindFirst(Constants.ObjectIdClaimType).Value).GetMSALCache();
                var client = new ConfidentialClientApplication(B2CAuthenticationOptions.ClientId,
                                                               B2CAuthenticationOptions.GetAuthority(principal.FindFirst(Constants.AcrClaimType).Value),
                                                               "https://app", // it's not really needed
                                                               new ClientCredential(B2CAuthenticationOptions.ClientSecret),
                                                               userTokenCache,
                                                               null);

                var result = await client.AcquireTokenByAuthorizationCodeAsync(context.TokenEndpointRequest.Code,
                    new[] { B2CAuthenticationOptions.ApiScopes });

                context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                /*20190214#GT#Gestione nuovi utenti*/
            }
            catch (Exception ex)
            {
                context.Fail(ex);
            }
        }

        private static Task OnMessageReceivedHandler(MessageReceivedContext context)
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
                    context.Response.Redirect("/accounts/reset-password");
                }
            }
            return Task.CompletedTask;
        }

        private static async Task OnRedirectToIdentityProviderHandler(RedirectContext context)
        {
            var configuration = await GetOpenIdConnectConfigurationAsync(context, B2CAuthenticationOptions.Policies.SignInOrSignUpPolicy);
            context.ProtocolMessage.IssuerAddress = configuration.AuthorizationEndpoint;
        }
        private static async Task OnRedirectToIdentityProviderForSignOutHandler(RedirectContext context)
        {
            var configuration = await GetOpenIdConnectConfigurationAsync(context, B2CAuthenticationOptions.Policies.SignInOrSignUpPolicy);
            context.ProtocolMessage.IssuerAddress = configuration.AuthorizationEndpoint;
        }

        private static Task OnRemoteFailureHandler(RemoteFailureContext ctx)
        {
            return Task.CompletedTask;
        }
        private static Task OnRemoteSignOutHandler(RemoteSignOutContext ctx)
        {
            return Task.CompletedTask;
        }
        private static Task OnSignedOutCallbackRedirectHandler(RemoteSignOutContext ctx)
        {
            return Task.CompletedTask;
        }
        private static Task OnTicketReceivedHandler(TicketReceivedContext ctx)
        {
            return Task.CompletedTask;
        }
        private static Task OnTokenResponseReceivedHandler(TokenResponseReceivedContext ctx)
        {
            return Task.CompletedTask;
        }
        private static Task OnTokenValidatedHandler(TokenValidatedContext ctx)
        {
            return Task.CompletedTask;
        }
        private static Task OnUserInformationReceivedHandler(UserInformationReceivedContext ctx)
        {
            return Task.CompletedTask;
        }
        #endregion

        private static Task<OpenIdConnectConfiguration> GetOpenIdConnectConfigurationAsync(RedirectContext context, string defaultPolicy)
        {
            var manager = (PolicyConfigurationManager)context.Options.ConfigurationManager;
            var policy = context.Properties.Items.ContainsKey(Constants.B2CPolicy) ? context.Properties.Items[Constants.B2CPolicy] : defaultPolicy;

            return manager.GetConfigurationByPolicyAsync(CancellationToken.None, policy);
        }
    }
}
