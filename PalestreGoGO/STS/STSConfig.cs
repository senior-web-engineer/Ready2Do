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
            return new List<IdentityResource>
            {
                new IdentityResources.OpenId(),
                new IdentityResources.Profile(),
            };
        }

        public static IEnumerable<ApiResource> GetApiResources()
        {
            return new List<ApiResource>
            {
                new ApiResource("usersManagement", "Gestione Utenti")
                {
                    Scopes =
                    {
                        new Scope("usersmanagementscope", "Scope per la gestione degli utenti")
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
