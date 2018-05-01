using IdentityModel;
using IdentityServer4.Extensions;
using IdentityServer4.Services;
using IdentityServer4.Stores;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Options;
using Palestregogo.STS.Model;
using Palestregogo.STS.Services;
using Palestregogo.STS.UI.Model;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using IdentityServer4.Validation;
using IdentityServer4.Models;

namespace Palestregogo.STS.UI.Services
{
    public class CustomRedirectUriValidator : IRedirectUriValidator
    {
        public Task<bool> IsPostLogoutRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (string.IsNullOrEmpty(requestedUri)) { return Task.FromResult(false); }
            if (client.RedirectUris == null) { return Task.FromResult(false); }
            if (client.RedirectUris.Count == 0) { return Task.FromResult(false); }
            foreach (var uri in client.RedirectUris)
            {
                if (requestedUri.StartsWith(requestedUri))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }

        public Task<bool> IsRedirectUriValidAsync(string requestedUri, Client client)
        {
            if (string.IsNullOrEmpty(requestedUri)) { return Task.FromResult(false); }
            if (client.RedirectUris == null) { return Task.FromResult(false); }
            if (client.RedirectUris.Count == 0) { return Task.FromResult(false); }
            foreach (var uri in client.RedirectUris)
            {
                if (requestedUri.StartsWith(requestedUri))
                {
                    return Task.FromResult(true);
                }
            }
            return Task.FromResult(false);
        }
    }
}
