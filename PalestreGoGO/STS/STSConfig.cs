using IdentityServer4.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
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
                claimTypes: new[] { "name", "email" /*INTEGRARE*/}
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
                new ApiResource("palestregogo.api")
                {
                    ApiSecrets= new List<Secret>{new Secret("f5d4fe477bd8462777829d1bff33869e34932b7415c760c0461d863eab8a7670".Sha256()) },
                    Scopes = new List<Scope>
                    {
                        new Scope("palestregogo.api.clienti.provisioning", "Provisioning Nuovi Clienti"),
                        /*Scoper per l'amministrazione delle utenze (dei tenant di cui si è owner)*/
                        new Scope("palestregogo.api.users.management", "Gestione Utenti")
                        {
                            UserClaims = new List<string>
                            {

                            }
                        }
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
                    ClientName = "Provisioning Client",
                    ClientId="provisioning.client",
                    AllowedGrantTypes = GrantTypes.ClientCredentials,
                    ClientSecrets = new List<Secret>(){new Secret("b753a50a8966dccb0c99248d2aa1fe2d65a6dca43de88cc1a2".Sha256())},
                    RequireClientSecret = true,
                    AllowedScopes = { "alestregogo.sts.assignownership", "palestregogo.api.clienti.provisioning" },
                    AccessTokenType = AccessTokenType.Jwt

                },
                      new Client
                {
                    ClientName = "angularclient",
                    ClientId = "angularclient",
                    AccessTokenType = AccessTokenType.Reference,
                    AccessTokenLifetime = 300,// 330 seconds, default 60 minutes
                    IdentityTokenLifetime = 20,
                    AllowedGrantTypes = GrantTypes.Implicit,
                    AllowAccessTokensViaBrowser = true,
                    RedirectUris = new List<string>{
                        "https://localhost:44387/"
                    },
                    PostLogoutRedirectUris = new List<string>
                    {
                        "https://localhost:44387/unauthorized",
                        "https://localhost:44387"
                    },
                    AllowedCorsOrigins = new List<string>
                    {
                        "https://localhost:44387",
                        "http://localhost:44386"
                    },
                    AllowedScopes = new List<string>
                    {
                        "openid",
                        //"dataEventRecords",
                        //"dataeventrecordsscope",
                        //"securedFiles",
                        //"securedfilesscope",
                        "role",
                        "profile",
                        "email"
                    }
                }
            };
        }
    }
}
