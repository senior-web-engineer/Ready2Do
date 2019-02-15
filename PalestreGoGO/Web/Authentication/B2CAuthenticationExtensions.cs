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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Web.Authentication
{
    public static class B2CAuthenticationExtensions
    {
        private static B2CAuthenticationOptions B2CAuthenticationOptions;
        private static IDistributedCache DistributedCache;

        public static void AddAzureAdB2Authentication(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();

            DistributedCache = serviceProvider.GetService<IDistributedCache>();
            services.AddSingleton(DistributedCache);

            B2CAuthenticationOptions = serviceProvider.GetService<IOptions<B2CAuthenticationOptions>>().Value;
            //var b2cPolicies = serviceProvider.GetService<IOptions<B2CPolicies>>();

            services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = Constants.OpenIdConnectAuthenticationScheme;
            })
            .AddCookie()
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
                    context.Response.Redirect("/Account/ResetPassword");
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
