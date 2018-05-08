using IdentityServer4;
using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace Palestregogo.STS
{
    public static class STSConfig
    {
        public static IConfiguration Configuration { get; set; }

        // scopes define the resources in your system
        public static IEnumerable<IdentityResource> GetIdentityResources()
        {
            //Referenziato tramite lo scope dal client
            var customProfile = new IdentityResource(
                name: "customprofile",
                displayName: "Profilo Completo",
                claimTypes: new[] { "name", "email", "role", Constants.CLAIMTYPE_STRUCTURE_OWNED, Constants.CLAIMTYPE_STRUCTURE_MANAGED }
                );

            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),     // Standard OIDC id_token
                new IdentityResources.Profile(),    // Standard OIDC profile
                customProfile
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("palestregogo.api", "palestregogo", new string[]{ Constants.CLAIMTYPE_STRUCTURE_MANAGED, Constants.CLAIMTYPE_STRUCTURE_OWNED })
                {
                    ApiSecrets= new List<Secret>{new Secret("f5d4fe477bd8462777829d1bff33869e34932b7415c760c0461d863eab8a7670".Sha256()) },
                    Scopes = new List<Scope>
                    {
                        //new Scope("palestregogo.api.clienti.provisioning", "Provisioning Nuovi Clienti"),
                        ///*Scoper per l'amministrazione delle utenze (dei tenant di cui si è owner)*/
                        //new Scope("palestregogo.api.users.management", "Gestione Utenti")
                        //{
                        //},
                        new Scope("palestregogo.api", "Accesso completo alle API")
                    }
                },


                new ApiResource("palestregogo.sts", "Gestione Utenti")
                {
                    Scopes =
                    {
                        new Scope("palestregogo.sts.write", "Creazione e modifica degli utenti"),
                        new Scope("palestregogo.sts.read", "Creazione e modifica degli utenti"),
                        new Scope("palestregogo.sts.assignownership", "Consente di impostare un utente come owner di un cliente"),
                    }
                }
            };
        }

        // clients want to access resources (aka scopes)
        public static IEnumerable<Client> GetClients()
        {
            // client credentials client
            return new List<Client>
            {
                //Client per il provisioning
                new Client
                {
                    ClientName = "Web Client",
                    ClientId="palestregogo_web",
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowOfflineAccess = true, //Enable Refresh Token (??)
                    ClientSecrets = new List<Secret>(){new Secret("b753a50a8966dccb0c99248d2aa1fe2d65a6dca43de88cc1a2".Sha256())},
                    RequireClientSecret = true,
                    AllowedScopes = { "palestregogo.api",
                                      "customprofile",
                                      IdentityServerConstants.StandardScopes.OpenId,
                                      IdentityServerConstants.StandardScopes.Email,
                                      IdentityServerConstants.StandardScopes.Profile,
                                      IdentityServerConstants.StandardScopes.OfflineAccess
                    },
                    AccessTokenType = AccessTokenType.Jwt,
#if DEBUG
                    RedirectUris = {"http://localhost:18071/signin-oidc", "https://localhost:44320/signin-oidc", "https://localhost:44320", "https://localhost:44320/" },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:44320/",
                        "http://localhost:18071"
                    },
#elif RELEASE
                    RedirectUris = {"https://gianlucatofi.it/signin-oidc", "https://gianlucatofi.it", "https://gianlucatofi.it/", "https://www.gianlucatofi.it", "https://www.gianlucatofi.it/" },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://gianlucatofi.it/",
                        "https://www.gianlucatofi.it/"
                    },
#endif
                    AllowAccessTokensViaBrowser = true, //permit transmit access tokens via the browser channel
                    RequireConsent = false,  //disabilitiamo il CONSENT SCREEN
                    AlwaysIncludeUserClaimsInIdToken = true,
                    AlwaysSendClientClaims = true,
                    
                }
#if DEBUG                
                ,new Client
                {
                    ClientName = "Test Client",
                    ClientId="tests.client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret>(){new Secret("01c33638af65289f0f998e439b744b91eedd20fe059095747b".Sha256())},
                    RequireClientSecret = true,
                    AllowedScopes = { "palestregogo.sts.assignownership", "openid", "role", "profile", "email", "offline_access"},
                    AccessTokenType = AccessTokenType.Jwt,
                }
#endif
            };
        }
    }
}
