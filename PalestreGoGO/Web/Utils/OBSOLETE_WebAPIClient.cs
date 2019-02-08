//using Common.Utils;
//using Microsoft.AspNetCore.Http;
//using Microsoft.Extensions.Caching.Distributed;
//using Microsoft.Extensions.Options;
//using Microsoft.Identity.Client;
//using Newtonsoft.Json;
//using PalestreGoGo.WebAPIModel;
//using ready2do.model.common;
//using Serilog;
//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using System.IO;
//using System.Linq;
//using System.Net.Http;
//using System.Net.Http.Headers;
//using System.Text;
//using System.Threading.Tasks;
//using System.Web;
//using Web.Authentication;
//using Web.Configuration;
//using Web.Models;
//using Web.Models.Mappers;
//using Web.Models.Utils;

//namespace Web.Utils
//{
//    public class WebAPIClient
//    {
//        private readonly static JsonSerializerSettings _serializerSettings;

//        private readonly AppConfig _appConfig;
//        private readonly IHttpContextAccessor _httpContextAccessor;
//        private readonly IDistributedCache _distributedCache;
//        private readonly B2CAuthenticationOptions _authOptions;
//        /// <summary>
//        /// Usiamo una unica istanza statica 
//        /// </summary>
//        private static HttpClient sClient = new HttpClient();

//        static WebAPIClient()
//        {
//            _serializerSettings = new JsonSerializerSettings();
//        }

//        public WebAPIClient(IOptions<AppConfig> options, IHttpContextAccessor httpContextAccessor,
//            IDistributedCache distributedCache, IOptions<B2CAuthenticationOptions> authOptions)
//        {
//            _appConfig = options?.Value;
//            _httpContextAccessor = httpContextAccessor;
//            _distributedCache = distributedCache;
//            _authOptions = authOptions.Value;
//        }

//        #region PRIVATE STUFF
//        private async Task<string> GetAccessTokenAsync()
//        {
//            try
//            {
//                var principal = _httpContextAccessor.HttpContext.User;

//                var tokenCache = new DistributedTokenCache(_distributedCache, principal.FindFirst(Constants.ObjectIdClaimType).Value).GetMSALCache();
//                var client = new ConfidentialClientApplication(_authOptions.ClientId,
//                                                          _authOptions.GetAuthority(principal.FindFirst(Constants.AcrClaimType).Value),
//                                                          "https://app", // it's not really needed
//                                                          new ClientCredential(_authOptions.ClientSecret),
//                                                          tokenCache,
//                                                          null);

//                var result = await client.AcquireTokenSilentAsync(new[] { _authOptions.ApiScopes },
//                                        (await client.GetAccountsAsync()).FirstOrDefault());

//                return result.AccessToken;
//            }
//            catch (MsalUiRequiredException)
//            {
//                throw new ReauthenticationRequiredException();
//            }
//        }
//        private async Task SendPostRequestAsync<T>(string uri, T model, bool sendToken = true)
//        {
//            var response = await SendRequestAsync<T>(HttpMethod.Post, new Uri(uri), model, sendToken);
//            response.Dispose();
//        }
//        private async Task SendPutRequestAsync<T>(string uri, T model, bool sendToken = true)
//        {
//            var response = await SendRequestAsync<T>(HttpMethod.Put, new Uri(uri), model, sendToken);
//            response.Dispose();
//        }

//        private async Task<HttpResponseMessage> SendRequestAsync<T>(HttpMethod method, Uri uri, T model, bool sendToken = true)
//        {
//            string accessToken = null;
//            if (sendToken) { accessToken = await this.GetAccessTokenAsync(); }
//            Log.Information("Invocazione API [{uri}], Method: {method}", uri, method);
//            Log.Debug("AccessToken: {accessToken}", accessToken);
//            Log.Debug("Model: {@model}", model);
//            var request = new HttpRequestMessage(method, uri);
//            if (!string.IsNullOrWhiteSpace(accessToken))
//            {
//                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//            }
//            try
//            {
//                if (model != null)
//                {
//                    using (var content = new PushStreamContent((stream, httpContent, transportContext) =>
//                                {
//                                    using (var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024))
//                                    {
//                                        using (var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.None })
//                                        {
//                                            var serializer = new JsonSerializer();
//                                            serializer.Serialize(writer, model);
//                                        }
//                                    }
//                                }, new MediaTypeHeaderValue("application/json")))
//                    {
//                        //Usiamo uno stream per serializzare ed inviare i dati
//                        request.Content = content;
//                        var response = await sClient.SendAsync(request);
//                        response.EnsureSuccessStatusCode();
//                        return response;
//                    }
//                }
//                else
//                {
//                    request.Content = new StringContent(""); //Empty content
//                    var response = await sClient.SendAsync(request);
//                    response.EnsureSuccessStatusCode();
//                    return response;
//                }
//            }
//            catch (Exception exc)
//            {
//                Log.Error(exc, "Errore durante l'invocazione della API: {uri}, Method: {method}, Model: {model}", uri, method, model);
//                throw;
//            }
//        }

//        private async Task<T> GetRequestAsync<T>(Uri uri, bool sendToken = true)
//        {
//            string accessToken = null;
//            if (sendToken) { accessToken = await this.GetAccessTokenAsync(); }
//            Log.Information("Invocazione API [{uri}]", uri);
//            Log.Debug("AccessToken: {accessToken}", accessToken);
//            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
//            T result = default(T);
//            if (!string.IsNullOrWhiteSpace(accessToken))
//            {
//                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//            }
//            try
//            {
//                using (HttpResponseMessage response = await sClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
//                {
//                    response.EnsureSuccessStatusCode();
//                    using (var streamResp = await response.Content.ReadAsStreamAsync())
//                    {
//                        using (var sr = new StreamReader(streamResp))
//                        {
//                            using (var jsonReader = new JsonTextReader(sr))
//                            {
//                                JsonSerializer serializer = new JsonSerializer();
//                                result = serializer.Deserialize<T>(jsonReader);
//                            }
//                        }
//                    }
//                }
//            }
//            catch (Exception exc)
//            {
//                Log.Error(exc, "Errore durante l'invocazione della API: {uri}", uri);
//                throw;
//            }
//            return result;
//        }

//        private async Task DeleteRequestAsync(string uri, bool sendToken = true)
//        {
//            string accessToken = null;
//            if (sendToken) { accessToken = await this.GetAccessTokenAsync(); }
//            Log.Information("Invocazione API [{uri}]", uri);
//            Log.Debug("AccessToken: {accessToken}", accessToken);
//            try
//            {
//                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
//                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
//                HttpResponseMessage response = await sClient.SendAsync(request);
//                response.EnsureSuccessStatusCode();
//            }
//            catch (Exception exc)
//            {
//                Log.Error(exc, "Errore durante l'invocazione della API: {uri}", uri);
//                throw;
//            }

//        }

//        #endregion




   


       
       
        
//    }
//}
