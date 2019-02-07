using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using System.Security.Claims;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Identity.Client;
using Web.Models;
using Newtonsoft.Json;
using System.Text;
using System.Runtime.Serialization.Formatters.Binary;
using Common.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using System.Web;
using Serilog;

namespace Web
{
    public static class AzureAdB2CAuthenticationBuilderExtensions
    {
        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder)
            => builder.AddAzureAdB2C(_ =>
            {
            });

        public static AuthenticationBuilder AddAzureAdB2C(this AuthenticationBuilder builder, Action<AzureAdB2COptions> configureOptions)
        {
            builder.Services.Configure(configureOptions);
            builder.Services.AddSingleton<IConfigureOptions<OpenIdConnectOptions>, OpenIdConnectOptionsSetup>();
            builder.AddOpenIdConnect();
            return builder;
        }

        public class OpenIdConnectOptionsSetup : IConfigureNamedOptions<OpenIdConnectOptions>
        {

            public OpenIdConnectOptionsSetup(IOptions<AzureAdB2COptions> b2cOptions)
            {
                AzureAdB2COptions = b2cOptions.Value;
            }

            public AzureAdB2COptions AzureAdB2COptions { get; set; }

            public void Configure(string name, OpenIdConnectOptions options)
            {
                options.ClientId = AzureAdB2COptions.ClientId;
                options.Authority = AzureAdB2COptions.GetAuthorityB2C(AzureAdB2COptions.DefaultPolicy);
                options.SaveTokens = true;
                options.UseTokenLifetime = true;
                options.TokenValidationParameters = new TokenValidationParameters() { NameClaimType = "name" };

                options.Events = new OpenIdConnectEvents()
                {
                    OnRedirectToIdentityProvider = OnRedirectToIdentityProvider,
                    OnRemoteFailure = OnRemoteFailure,
                    OnAuthorizationCodeReceived = OnAuthorizationCodeReceived,
                    OnAuthenticationFailed = OnAuthenticationFailed
                };
            }

            public void Configure(OpenIdConnectOptions options)
            {
                Configure(Options.DefaultName, options);
            }

            public Task OnAuthenticationFailed(AuthenticationFailedContext context)
            {
                Log.Error(context.Exception, "Authentication Failed {context}", context);
                context.Response.Redirect("/Home/Error?message=" + HttpUtility.UrlEncode(context.Exception.Message));
                return Task.FromResult(0);
            }

            public Task OnRedirectToIdentityProvider(RedirectContext context)
            {
                if(!context.Properties.Items.TryGetValue(AzureAdB2COptions.PolicyAuthenticationProperty, out var policy))
                {
                    policy = AzureAdB2COptions.DefaultPolicy;
                }

                context.ProtocolMessage.State = BuildProtocolState(context);

                context.ProtocolMessage.Scope = $"{OpenIdConnectScope.OpenId} offline_access {AzureAdB2COptions.ApiScopes}";
                context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                context.ProtocolMessage.IssuerAddress = context.ProtocolMessage.IssuerAddress.ToLower().Replace(AzureAdB2COptions.DefaultPolicy.ToLower(), policy.ToLower());
                context.Properties.Items.Remove(AzureAdB2COptions.PolicyAuthenticationProperty);
                //#GT#20190207#Modifica con nuova policy unica di signup/signip per clienti ed utenti
                //context.ProtocolMessage.Scope += $" offline_access {AzureAdB2COptions.ApiScopes}";
                //context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                //if (!string.IsNullOrEmpty(AzureAdB2COptions.ApiUrl) && (
                //    policy.Equals(AzureAdB2COptions.StrutturaSignInPolicyId, StringComparison.InvariantCultureIgnoreCase) ||
                //    policy.Equals(AzureAdB2COptions.UserSignUpSignInPolicyId, StringComparison.InvariantCultureIgnoreCase)))
                //{
                //    context.ProtocolMessage.Scope += $" offline_access {AzureAdB2COptions.ApiScopes}";
                //    context.ProtocolMessage.ResponseType = OpenIdConnectResponseType.CodeIdToken;
                //}
                return Task.FromResult(0);
            }

            public Task OnRemoteFailure(RemoteFailureContext context)
            {
                context.HandleResponse();
                Log.Error("Remote Authentication Error {context}", context);
                // Handle the error code that Azure AD B2C throws when trying to reset a password from the login page 
                // because password reset is not supported by a "sign-up or sign-in policy"
                if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("AADB2C90118"))
                {
                    // If the user clicked the reset password link, redirect to the reset password route
                    context.Response.Redirect("/Session/ResetPassword");
                }
                else if (context.Failure is OpenIdConnectProtocolException && context.Failure.Message.Contains("access_denied"))
                {
                    context.Response.Redirect("/");
                }
                else
                {
                    context.Response.Redirect("/Home/Error?message=" + HttpUtility.UrlEncode(context.Failure.Message));
                }
                return Task.FromResult(0);
            }

            public async Task OnAuthorizationCodeReceived(AuthorizationCodeReceivedContext context)
            {
                // Use MSAL to swap the code for an access token
                // Extract the code from the response notification
                var code = context.ProtocolMessage.Code;

                string signedInUserID = context.Principal.FindFirst(ClaimTypes.NameIdentifier).Value;

                //GT#20181007#Recuperiamo il claim
                string claimValue = context.Principal.FindFirstValue("extension_accountConfirmedOn");                
                string currentPolicy = context.Principal.FindFirstValue(Constants.ClaimPolicy);
                TokenCache userTokenCache = new MSALSessionCache(signedInUserID, context.HttpContext).GetMsalCacheInstance();
                //La versione 2.7.0 di MSAL ha dei problemi con il nuovo
                string authorityMSLogin = AzureAdB2COptions.GetAuthorityB2C(currentPolicy);
                ConfidentialClientApplication cca = new ConfidentialClientApplication(AzureAdB2COptions.ClientId, authorityMSLogin, AzureAdB2COptions.RedirectUri, new ClientCredential(AzureAdB2COptions.ClientSecret), userTokenCache, null);
                cca.ValidateAuthority = false;
                try
                {
                    //IEnumerable<string> scopes = new string[] { "https://ready2do.onmicrosoft.com/api/api_all" };
                    
                    AuthenticationResult result = await cca.AcquireTokenByAuthorizationCodeAsync(code, AzureAdB2COptions.ApiScopes.Split(' '));


                    context.HandleCodeRedemption(result.AccessToken, result.IdToken);
                }
                catch (Exception exc)
                {
                    Log.Error(exc, "Errore durante la richiesta del authorization_code");
                    //TODO: Handle
                    throw;
                }
                this.HandleStateOnReturn(context);
                //TODO: Gestire il redirect alla struttura gestita se il redirect è alla Home del sito (solo per i gestori ?)
                if(!string.IsNullOrEmpty(context.Properties.RedirectUri)) //&& (!IsRedirectToHome(context.Properties.RedirectUri)))
                {
                    //TODO: Verificare se sia corretta questa gestione e se espone a vulerabilità (open redirect)
                    context.Response.Redirect(context.Properties.RedirectUri);
                }
            }

            /// <summary>
            /// Ritorna il valore del parametro state da passare a B2C
            /// </summary>
            /// <param name="context"></param>
            /// <returns></returns>
            private string BuildProtocolState(RedirectContext context)
            {
                OpenIdStateInfo state = new OpenIdStateInfo();
                if (context.Properties.Items.TryGetValue("SignupType", out var signupType))
                {
                    state.SignupType = signupType;
                }
                else
                {
                    state.Id =  Guid.NewGuid();
                }
                return state.ToBase64Json();
            }

            private void HandleStateOnReturn(AuthorizationCodeReceivedContext context)
            {
                var state = context.ProtocolMessage.State;
                var stateInfo = state.FromBase64Json<OpenIdStateInfo>();
                if (!string.IsNullOrWhiteSpace(stateInfo.SignupType) && stateInfo.SignupType.Equals("Cliente"))
                {
                    context.Response.Redirect("register");
                    return;
                }
                }

        }
    }

    internal class OpenIdStateInfo
    {
        [JsonProperty(PropertyName = "id", NullValueHandling = NullValueHandling.Ignore)]
        public Guid? Id { get; set; }

        [JsonProperty(PropertyName ="st", NullValueHandling =NullValueHandling.Ignore)]
        public string SignupType { get; set; }
    }
}
