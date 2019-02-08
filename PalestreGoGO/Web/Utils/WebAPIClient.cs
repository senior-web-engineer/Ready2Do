using Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Microsoft.Identity.Client;
using Newtonsoft.Json;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Web.Authentication;
using Web.Configuration;
using Web.Models;
using Web.Models.Mappers;
using Web.Models.Utils;

namespace Web.Utils
{
    public class WebAPIClient
    {
        private readonly static JsonSerializerSettings _serializerSettings;

        private readonly AppConfig _appConfig;
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IDistributedCache _distributedCache;
        private readonly B2CAuthenticationOptions _authOptions;
        /// <summary>
        /// Usiamo una unica istanza statica 
        /// </summary>
        private static HttpClient sClient = new HttpClient();

        static WebAPIClient()
        {
            _serializerSettings = new JsonSerializerSettings();
        }

        public WebAPIClient(IOptions<AppConfig> options, IHttpContextAccessor httpContextAccessor,
            IDistributedCache distributedCache, IOptions<B2CAuthenticationOptions> authOptions)
        {
            _appConfig = options?.Value;
            _httpContextAccessor = httpContextAccessor;
            _distributedCache = distributedCache;
            _authOptions = authOptions.Value;
        }

        #region PRIVATE STUFF
        private async Task<string> GetAccessTokenAsync()
        {
            try
            {
                var principal = _httpContextAccessor.HttpContext.User;

                var tokenCache = new DistributedTokenCache(_distributedCache, principal.FindFirst(Constants.ObjectIdClaimType).Value).GetMSALCache();
                var client = new ConfidentialClientApplication(_authOptions.ClientId,
                                                          _authOptions.GetAuthority(principal.FindFirst(Constants.AcrClaimType).Value),
                                                          "https://app", // it's not really needed
                                                          new ClientCredential(_authOptions.ClientSecret),
                                                          tokenCache,
                                                          null);

                var result = await client.AcquireTokenSilentAsync(new[] { _authOptions.ApiScopes },
                                        (await client.GetAccountsAsync()).FirstOrDefault());

                return result.AccessToken;
            }
            catch (MsalUiRequiredException)
            {
                throw new ReauthenticationRequiredException();
            }
        }
        private async Task SendPostRequestAsync<T>(string uri, T model, bool sendToken = true)
        {
            var response = await SendRequestAsync<T>(HttpMethod.Post, new Uri(uri), model, sendToken);
            response.Dispose();
        }
        private async Task SendPutRequestAsync<T>(string uri, T model, bool sendToken = true)
        {
            var response = await SendRequestAsync<T>(HttpMethod.Put, new Uri(uri), model, sendToken);
            response.Dispose();
        }

        private async Task<HttpResponseMessage> SendRequestAsync<T>(HttpMethod method, Uri uri, T model, bool sendToken = true)
        {
            string accessToken = null;
            if (sendToken) { accessToken = await this.GetAccessTokenAsync(); }
            Log.Information("Invocazione API [{uri}], Method: {method}", uri, method);
            Log.Debug("AccessToken: {accessToken}", accessToken);
            Log.Debug("Model: {@model}", model);
            var request = new HttpRequestMessage(method, uri);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            try
            {
                if (model != null)
                {
                    using (var content = new PushStreamContent((stream, httpContent, transportContext) =>
                                {
                                    using (var writer = new StreamWriter(stream, new UTF8Encoding(false), 1024))
                                    {
                                        using (var jsonWriter = new JsonTextWriter(writer) { Formatting = Formatting.None })
                                        {
                                            var serializer = new JsonSerializer();
                                            serializer.Serialize(writer, model);
                                        }
                                    }
                                }, new MediaTypeHeaderValue("application/json")))
                    {
                        //Usiamo uno stream per serializzare ed inviare i dati
                        request.Content = content;
                        var response = await sClient.SendAsync(request);
                        response.EnsureSuccessStatusCode();
                        return response;
                    }
                }
                else
                {
                    request.Content = new StringContent(""); //Empty content
                    var response = await sClient.SendAsync(request);
                    response.EnsureSuccessStatusCode();
                    return response;
                }
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Errore durante l'invocazione della API: {uri}, Method: {method}, Model: {model}", uri, method, model);
                throw;
            }
        }

        private async Task<T> GetRequestAsync<T>(Uri uri, bool sendToken = true)
        {
            string accessToken = null;
            if (sendToken) { accessToken = await this.GetAccessTokenAsync(); }
            Log.Information("Invocazione API [{uri}]", uri);
            Log.Debug("AccessToken: {accessToken}", accessToken);
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            T result = default(T);
            if (!string.IsNullOrWhiteSpace(accessToken))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
            }
            try
            {
                using (HttpResponseMessage response = await sClient.SendAsync(request, HttpCompletionOption.ResponseHeadersRead))
                {
                    response.EnsureSuccessStatusCode();
                    using (var streamResp = await response.Content.ReadAsStreamAsync())
                    {
                        using (var sr = new StreamReader(streamResp))
                        {
                            using (var jsonReader = new JsonTextReader(sr))
                            {
                                JsonSerializer serializer = new JsonSerializer();
                                result = serializer.Deserialize<T>(jsonReader);
                            }
                        }
                    }
                }
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Errore durante l'invocazione della API: {uri}", uri);
                throw;
            }
            return result;
        }

        private async Task DeleteRequestAsync(string uri, bool sendToken = true)
        {
            string accessToken = null;
            if (sendToken) { accessToken = await this.GetAccessTokenAsync(); }
            Log.Information("Invocazione API [{uri}]", uri);
            Log.Debug("AccessToken: {accessToken}", accessToken);
            try
            {
                HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, uri);
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", accessToken);
                HttpResponseMessage response = await sClient.SendAsync(request);
                response.EnsureSuccessStatusCode();
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Errore durante l'invocazione della API: {uri}", uri);
                throw;
            }

        }

        #endregion

        #region CLIENTI
        public async Task<bool> CheckUrlRoute(string urlRoute, int? idCliente = null)
        {
            string tmpqs = idCliente.HasValue ? $"&idCliente={idCliente}" : "";
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/checkurl?url={urlRoute}{tmpqs}");
            return await GetRequestAsync<bool>(uri, null);
        }

        public async Task<bool> NuovoClienteAsync(NuovoClienteAPIModel cliente)
        {
            try
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti", cliente, null);
                return true;
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Errore durante la creazione del cliente {cliente}.", cliente);
                return false;
            }
        }

        public async Task<ClienteDM> GetClienteAsync(int idCliente)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}";
            return await GetRequestAsync<ClienteDM>(new Uri(uri), null);
        }

        public async Task<ClienteDM> GetClienteAsync(string urlRoute)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{urlRoute}";
            return await GetRequestAsync<ClienteDM>(new Uri(uri), null);
        }

        public async Task ClienteSalvaProfilo(int idCliente, ClienteProfiloAPIModel profilo)
        {
            await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo", profilo, true);
        }

        public async Task ClienteSalvaBanner(int idCliente, ImmagineClienteInputDM banner)
        {
            if (banner.IdTipoImmagine != (int)TipoImmagineDM.Sfondo) { throw new ArgumentException(nameof(banner)); }
            if (!banner.Id.HasValue) { throw new ArgumentException(nameof(banner)); }
            await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti{idCliente:int}/images", banner, true);
        }

        public async Task GallerySalvaImmagine(int idCliente, ImmagineClienteInputDM image)
        {
            if (image.Id.HasValue && image.Id.Value > 0)
            {
                await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images/{image.Id}", image, true);
            }
            else
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images", image, true);
            }
        }

        public async Task<ImmagineClienteDM> DeleteImmagineGalleryAsync(int idCliente, int idImage)
        {
            //Facciamo una DELETE custom perchè ci serve l'url ritornato (eccezionalmente) per poter cancellare il file dallo Storage
            string access_token = await GetAccessTokenAsync();
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images/{idImage}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            using (HttpResponseMessage response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ImmagineClienteDM>(responseString, _serializerSettings);
            }
        }

        public async Task<IEnumerable<ImmagineClienteDM>> GetImmaginiClienteAsync(int idCliente, TipoImmagineDM tipoImmagini, bool sendAuthToken = true)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images?tipo={tipoImmagini}");
            return await GetRequestAsync<IEnumerable<ImmagineClienteDM>>(uri, sendAuthToken);
        }

        public async Task ClienteSalvaOrarioApertura(int idCliente, OrarioAperturaDM orario)
        {
            await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo/orario", orario);
        }

        public async Task ClienteSalvaAnagrafica(int idCliente, AnagraficaClienteApiModel anagrafica, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo/anagrafica");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = new StringContent(JsonConvert.SerializeObject(anagrafica), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        #region TIPOLOGIE CLIENTE

        public async Task<IEnumerable<TipologiaClienteDM>> GetTipologieClientiAsync()
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/tipologie");
            return await GetRequestAsync<IEnumerable<TipologiaClienteDM>>(uri, false);
        }

        #endregion

        #region LOCATIONS

        public async Task<IEnumerable<LocationDM>> GetLocationsAsync(int idCliente)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations");
            return await GetRequestAsync<IEnumerable<LocationDM>>(uri, false);
        }

        public async Task SaveLocationAsync(int idCliente, LocationInputDM location)
        {
            if (location.Id.HasValue && location.Id.Value > 0)
            {
                await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{location.Id}", location);
            }
            else
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations", location);
            }
        }

        public async Task<LocationInputDM> GetOneLocationAsync(int idCliente, int idLocation)
        {
            return await GetRequestAsync<LocationInputDM>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{idLocation}"));
        }

        public async Task DeleteOneLocationAsync(int idCliente, int idLocation)
        {
            await DeleteRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{idLocation}");
        }
        #endregion

        #region TIPOLOGIE LEZIONI
        public async Task SaveTipologiaLezioneAsync(int idCliente, TipologiaLezioneDM tipoLezione)
        {
            if (tipoLezione.Id.HasValue && tipoLezione.Id.Value > 0)
            {
                await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni/{tipoLezione.Id}", tipoLezione);
            }
            else
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni", tipoLezione);
            }
        }

        public async Task<IEnumerable<TipologiaLezioneDM>> GetTipologieLezioniClienteAsync(int idCliente)
        {
            return await GetRequestAsync<IEnumerable<TipologiaLezioneDM>>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni"), false);
        }

        public async Task<TipologiaLezioneDM> GetOneTipologiaLezione(int idCliente, int idTipologia)
        {
            return await GetRequestAsync<TipologiaLezioneDM>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni/{idTipologia}"));
        }

        public async Task<bool> CheckNameTipologiaLezioneAsync(int idCliente, string nome, string access_token, int? id)
        {
            string queryString = id.HasValue ? $"?id={id}" : "";
            return await GetRequestAsync<bool>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni/checkname/{nome}{queryString}"), access_token);
        }

        public async Task DeleteOneTipologiaLezioneAsync(int idCliente, int idLocation)
        {
            await DeleteRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipolezioni/{idLocation}");
        }

        #endregion

        #region TIPOLOGIE ABBONAMENTI
        public async Task<IEnumerable<TipologiaAbbonamentoDM>> GetTipologieAbbonamentiClienteAsync(int idCliente)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti");
            return await GetRequestAsync<IEnumerable<TipologiaAbbonamentoDM>>(uri);
        }

        public async Task<TipologiaAbbonamentoDM> GetOneTipologiaAbbonamentoAsync(int idCliente, int idTipoAbbonamento)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti/{idTipoAbbonamento}");
            return await GetRequestAsync<TipologiaAbbonamentoDM>(uri);
        }

        public async Task SaveTipologiaAbbonamentoAsync(int idCliente, TipologiaAbbonamentoInputDM tipoAbbonamento)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti");
            if (tipoAbbonamento.Id.HasValue && tipoAbbonamento.Id.Value > 0)
            {
                await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti/{tipoAbbonamento.Id}", tipoAbbonamento);
            }
            else
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti", tipoAbbonamento);
            }
        }

        public async Task DeleteOneTipologiaAbbonamentoAsync(int idCliente, int idTipoAbbonamento)
        {
            await DeleteRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti/{idTipoAbbonamento}");
        }

        public async Task<bool> CheckNomeTipologiaAbbonamentoAsync(int idCliente, string nome, int? id)
        {
            string qsOpz = id.HasValue ? $"&id={id}" : "";
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/tipologiche/tipoabbonamenti/checknome?nome={nome}{qsOpz}");
            return await GetRequestAsync<bool>(uri);
        }

        #endregion

        #region SCHEDULES
        public async Task<IEnumerable<ScheduleDM>> GetSchedulesAsync(int idCliente, DateTime start, DateTime end, int? idLocation)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules?sd={start.ToISO8601(true)}&ed={end.ToISO8601(true)}&lid={idLocation}");
            return await GetRequestAsync<IEnumerable<ScheduleDM>>(uri, false);
        }

        public async Task<ScheduleDM> GetScheduleAsync(int idCliente, int idEvento)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}");
            return await GetRequestAsync<ScheduleDM>(uri, false);
        }

        public async Task SaveSchedule(int idCliente, ScheduleInputDM schedule)
        {
            if (schedule.Id.HasValue && schedule.Id.Value > 0)
            {
                await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{schedule.Id}", schedule);
            }
            else
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules", schedule);
            }
        }

        #endregion

        #region APPUNTAMENTI
        //Utilizzabile sia dagli Owner (ritorna tutti gli appuntamenti) che dall'utente (ritorna solamente il suo)
        public async Task<IEnumerable<AppuntamentoDM>> GetAppuntamentiForEventoAsync(int idCliente, int idEvento, string access_token)
        {
            var uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}/appuntamenti");
            return await GetRequestAsync<IEnumerable<AppuntamentoDM>>(uri, access_token);
        }

        public async Task<IEnumerable<AppuntamentoDaConfermareDM>> GetAppuntamentiDaConferamareForEventoAsync(int idCliente, int idEvento, string access_token)
        {
            var uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}/appuntamenti/unconfirmed");
            return await GetRequestAsync<IEnumerable<AppuntamentoDaConfermareDM>>(uri, access_token);
        }
        public async Task<IEnumerable<WaitListRegistrationDM>> GetWaitListRegistrationsForEventoAsync(int idCliente, int idEvento, string access_token)
        {
            var uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}/appuntamenti/waitlist");
            return await GetRequestAsync<IEnumerable<WaitListRegistrationDM>>(uri, access_token);
        }


        public async Task TakeAppuntamentoForCurrentUser(int idCliente, NuovoAppuntamentoApiModel appuntamento, string access_token)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/appuntamenti";
            await SendPostRequestAsync(uri, appuntamento, access_token);
        }

        /// <summary>
        /// Invocabile da un Utente per ottenere i propri appuntamenti
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<List<AppuntamentoUserApiModel>> GetAppuntamentiForCurrentUserAsync(string userId, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/{userId}/appuntamenti");
            return await GetRequestAsync<List<AppuntamentoUserApiModel>>(uri, access_token);
        }

        /// <summary>
        /// Invocabile da un Utente per ottenere i propri appuntamenti
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<List<AppuntamentoUserApiModel>> GetAppuntamentiForUserAsync(int idCliente, string userId, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/appuntamenti");
            return await GetRequestAsync<List<AppuntamentoUserApiModel>>(uri, access_token);
        }
        #endregion 

        ///// <summary>
        ///// Ritorna la lista dei clienti followed dall'utente corrente
        ///// </summary>
        ///// <param name="idCliente"></param>
        ///// <param name="access_token"></param>
        ///// <returns></returns>
        ///// 
        //public async Task<List<ClienteFollowed>> ClientiFollowedByUserAsync(string userId, string access_token)
        //{
        //    Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/{userId}/clientifollowed");
        //    HttpClient client = new HttpClient();
        //    HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
        //    request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
        //    HttpResponseMessage response = await client.SendAsync(request);
        //    response.EnsureSuccessStatusCode();
        //    var responseString = await response.Content.ReadAsStringAsync();
        //    var apiResult = JsonConvert.DeserializeObject<List<ClienteFollowedApiModel>>(responseString, _serializerSettings);
        //    return apiResult?.MapToEnumerableClienteFollowed().ToList();
        //}

        /// <summary>
        /// Verifica se un utente sia già un Follower di un determinato cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<bool> ClienteIsFollowedByUserAsync(int idCliente, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/follow/{idCliente}");
            return await GetRequestAsync<bool>(uri, access_token);
        }

        public async Task ClienteFollowAsync(int idCliente, string access_token)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/follow";
            await SendPostRequestAsync(uri, "", access_token);
        }

        public async Task ClienteUnFollowAsync(int idCliente, string access_token)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/unfollow";
            await SendPostRequestAsync(uri, "", access_token);
        }

        public async Task<List<ClienteUtenteViewModel>> GetUtentiCliente(int idCliente, string access_token, bool includeStatus, int page = 1, int pageSize = 2000, string sortBy = "Cognome", bool asc = true)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users?stato={includeStatus}&page={page}&pageSize={pageSize}&sortby={sortBy}&asc={asc}");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            if (!includeStatus)
            {
                var result = JsonConvert.DeserializeObject<IEnumerable<UtenteClienteDM>>(responseString, _serializerSettings);
                return result.MapToClienteUtenteViewModel();
            }
            else
            {
                var result = JsonConvert.DeserializeObject<IEnumerable<UtenteClienteDetailsDM>>(responseString, _serializerSettings);
                return result.MapToClienteUtenteViewModel();
            }
        }

        public async Task<ClienteUtenteDetailsApiModel> GetUtenteCliente(int idCliente, string userId)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}");
            return await GetRequestAsync<ClienteUtenteDetailsApiModel>(uri);
        }

        public async Task DeleteAppuntamentoUserAsync(int idCliente, int idSchedule)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idSchedule}/appuntamenti";
            await DeleteRequestAsync(uri);
        }

        public async Task DeleteAppuntamentoAdminAsync(int idCliente, int idSchedule, int idAppuntamento)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idSchedule}/appuntamenti/{idAppuntamento}";
            await DeleteRequestAsync(uri);
        }


        #region NOTIFICHE
        public async Task<List<NotificaViewModel>> GetNotificheForUserAsync(string access_token, int? idCliente = null)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/users/notifiche");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<NotificaConTipoApiModel>>(responseString, _serializerSettings);
            return result.MapToViewModel();
        }
        #endregion

        #region ABBONAMENTI UTENTI-CLIENTI

        public async Task EditAbbonamentoClienteAsync(int idCliente, string userId, AbbonamentoUtenteInputDM abbonamento)
        {
            await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/abbonamenti/{userId}", abbonamento);
        }

        public async Task DeleteAbbonamentoClienteAsync(int idCliente, string userId, int idAbbonamento)
        {
            await DeleteRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/abbonamenti/{userId}/{idAbbonamento}");
        }

        public async Task<IEnumerable<AbbonamentoUtenteDM>> GetAbbonamentiForUserAsync(int idCliente, string userId, bool includeExpired = false, bool includeDeleted = false)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/abbonamenti/{userId}?incExp={includeExpired}&incDel={includeDeleted}");
            return await GetRequestAsync<IEnumerable<AbbonamentoUtenteDM>>(uri);
        }

        public async Task<AbbonamentoUtenteDM> GetAbbonamentoAsync(int idCliente, int idAbbonamento)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/abbonamenti/{idAbbonamento}");
            return await GetRequestAsync<AbbonamentoUtenteDM>(uri);
        }

        #endregion

        #region CERTIFICATI UTENTI-CLIENTI

        public async Task AddCertificatoUtenteClienteAsync(int idCliente, string userId, ClienteUtenteCertificatoApiModel certificato)
        {
            await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/certificati", certificato);
        }

        public async Task DeleteCertificatoUtenteClienteAsync(int idCliente, string userId, int idCertificato)
        {
            await DeleteRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/users/{userId}/certificati/{idCertificato}");
        }

        public async Task<IEnumerable<ClienteUtenteCertificatoApiModel>> GetCertificatiForUserAsync(int idCliente, string userId, bool includeExpired = false, bool includeDeleted = false)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/certificati?incExp={includeExpired}&incDel={includeDeleted}");
            return await GetRequestAsync<IEnumerable<ClienteUtenteCertificatoApiModel>>(uri);
        }

        #endregion

        #region UTENTI
        public async Task<bool> CheckEmail(string email)
        {
            return await GetRequestAsync<bool>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/checkemail?email={email}"), false);
        }

        public async Task<bool> NuovoUtenteAsync(NuovoUtenteViewModel utente, int? idStrutturaAffiliata)
        {
            //TODO: Rimuovere l'idStrutturaAffiliata dalla querystring e metterlo nel body
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti?idref={idStrutturaAffiliata}");
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}api/clienti");
            var content = new StringContent(JsonConvert.SerializeObject(utente), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(uri, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<UserConfirmationResultAPIModel> ConfermaAccount(string email, string code)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/confirmation?email={email}&code={code}");
            using (var response = await SendRequestAsync<UserConfirmationResultAPIModel>(HttpMethod.Post, uri, null, false))
            {
                return JsonConvert.DeserializeObject<UserConfirmationResultAPIModel>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<UtenteDM> GetProfiloUtente()
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/profilo");
            return await GetRequestAsync<UtenteDM>(uri);
        }

        public async Task SalvaProfiloUtente(UtenteInputDM profilo)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/utenti/profilo";
            await SendPutRequestAsync(uri, profilo);
        }

        #endregion
    }
}
