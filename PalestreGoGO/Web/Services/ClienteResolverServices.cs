using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Utils;

namespace Web.Services
{
    public class ClienteResolverServices
    {
        private const int DEFAULT_SLIDING_EXPIRATION = 60; //Un ora la sliding expiration di default
        private IMemoryCache _cache;
        private WebAPIClient _apiClient;
        private MemoryCacheConfig _cacheConfig;
        public ClienteResolverServices(IMemoryCache memCache,
                                        IOptions<AppConfig> options,
                                    WebAPIClient apiClient)
        {
            _cache = memCache;
            _apiClient = apiClient;
            _cacheConfig = options.Value.CacheConfig;
        }

        private TimeSpan GetSlidingExpirationInterval()
        {
            return new TimeSpan(0, _cacheConfig?.ClienteSlidingExpiration ?? DEFAULT_SLIDING_EXPIRATION, 0);
        }

        private string GetIdClienteKey(int idCliente)
        {
            return $"IdCliente-{idCliente}";
        }

        public int? TryGetIdClienteFromRoute(string clienteRoute)
        {
            int? idCliente;
            _cache.TryGetValue(clienteRoute, out idCliente);
            return idCliente;
        }

        public async Task<int> GetIdClienteFromRouteAsync(string clienteRoute)
        {
            if (string.IsNullOrWhiteSpace(clienteRoute)) return -1;
            int idCliente = -1;
            if (!_cache.TryGetValue(clienteRoute, out idCliente))
            {
                idCliente = (await _apiClient.GetClienteAsync(clienteRoute)).Id.Value;
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(GetSlidingExpirationInterval());
                _cache.Set(clienteRoute, idCliente, cacheEntryOptions);
                _cache.Set(GetIdClienteKey(idCliente), clienteRoute, cacheEntryOptions);
            }
            return idCliente;
        }
        public async Task<string> GetRouteClienteFromIdAsync(int idCliente)
        {
            string result;
            string key = GetIdClienteKey(idCliente);
            if (!_cache.TryGetValue(key, out result))
            {
                var cliente = await _apiClient.GetClienteAsync(idCliente);
                result = cliente.UrlRoute;
                var cacheEntryOptions = new MemoryCacheEntryOptions()
                        .SetSlidingExpiration(GetSlidingExpirationInterval());
                _cache.Set(result, idCliente, cacheEntryOptions);
                _cache.Set(key, result, cacheEntryOptions);
            }
            return result;
        }
    }
}
