using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models;
using Web.Models.Mappers;
using Web.Models.Utils;

namespace Web.Utils
{
    public class WebAPIClient
    {
        private readonly static JsonSerializerSettings _serializerSettings;

        private AppConfig _appConfig;
        static WebAPIClient()
        {
            _serializerSettings = new JsonSerializerSettings();
        }

        public WebAPIClient(IOptions<AppConfig> options)
        {
            _appConfig = options?.Value;
        }

        public async Task<bool> NuovoClienteAsync(NuovoClienteViewModel cliente)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti");
            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(uri, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> NuovoUtenteAsync(NuovoUtenteViewModel utente, int? idStrutturaAffiliata)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti?idref={idStrutturaAffiliata}");
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}api/clienti");
            var content = new StringContent(JsonConvert.SerializeObject(utente), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(uri, content);
            return response.IsSuccessStatusCode;
        }

        public async Task<ClienteWithImagesViewModel> GetClienteAsync(int idCliente)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClienteWithImagesViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task<ClienteWithImagesViewModel> GetClienteAsync(string urlRoute)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{urlRoute}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound) { return null; }
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClienteWithImagesViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task<ClienteWithImagesViewModel> GetClienteFromTokenAsync(string securityToken)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/clienti/token/{securityToken}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClienteWithImagesViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task<IEnumerable<TipologiaClienteViewModel>> GetTipologiClientiAsync()
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/tipologie");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            //NOTA: Forziamo la deserializzazione al tipo "locale" e non quello originale nell'assemblu WebApiDataModel
            var result = JsonConvert.DeserializeObject<IEnumerable<TipologiaClienteViewModel>>(responseString, _serializerSettings);
            return result;
        }

        public async Task<IEnumerable<Models.LocationViewModel>> GetLocationsAsync(int idCliente)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<Models.LocationViewModel>>(responseString, _serializerSettings);
            return result;
        }

        public async Task SaveLocationAsync(int idCliente, Models.LocationViewModel location, string access_token)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations");
            HttpRequestMessage request;
            if (location.Id.HasValue && location.Id.Value > 0)
            {
                request = new HttpRequestMessage(HttpMethod.Put, uri);
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Post, uri);
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            //Sfruttiamo il fatto che i tipi di dati sono identici tra API e WEB per evitare di rimapparlo
            request.Content = new StringContent(JsonConvert.SerializeObject(location), Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }


        public async Task<Models.LocationViewModel> GetOneLocationAsync(int idCliente, int idLocation, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{idLocation}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Models.LocationViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task DeleteOneLocationAsync(int idCliente, int idLocation, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{idLocation}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<Models.TipologieLezioniViewModel>> GetTipologieLezioniClienteAsync(int idCliente)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipolezioni");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            //NOTA: Forziamo la deserializzazione al tipo "locale" e non quello originale nell'assembly WebApiDataModel
            var result = JsonConvert.DeserializeObject<IEnumerable<Models.TipologieLezioniViewModel>>(responseString, _serializerSettings);
            return result;
        }

        public async Task<Models.TipologieLezioniViewModel> GetOneTipologiaLezione(int idCliente, int idTipologia, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipolezioni/{idTipologia}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Models.TipologieLezioniViewModel>(responseString, _serializerSettings);
            return result;
        }


        //checkname/{nome:string}
        public async Task<bool> CheckNameTipologiaLezioneAsync(int idCliente, string nome, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipolezioni/checkname/{nome}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<bool>(responseString, _serializerSettings);
            return result;
        }


        public async Task ClienteSalvaProfilo(int idCliente, ClienteProfiloViewModel profilo, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = new StringContent(JsonConvert.SerializeObject(profilo), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task ClienteSalvaBanner(int idCliente, ImmagineViewModel banner, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo/banner");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = new StringContent(JsonConvert.SerializeObject(banner), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task ClienteSalvaOrarioApertura(int idCliente, PalestreGoGo.WebAPIModel.OrarioAperturaViewModel orario, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Put, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo/orario");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = new StringContent(JsonConvert.SerializeObject(orario), Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
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

        public async Task SaveTipologiaLezioneAsync(int idCliente, Models.TipologieLezioniViewModel tipoLezione, string access_token)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipolezioni");
            //Sfruttiamo il fatto che i tipi di dati sono identici tra API e WEB per evitare di rimapparlo
            HttpRequestMessage request;
            if (tipoLezione.Id.HasValue && tipoLezione.Id.Value > 0)
            {
                request = new HttpRequestMessage(HttpMethod.Put, uri);
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Post, uri);
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            //Sfruttiamo il fatto che i tipi di dati sono identici tra API e WEB per evitare di rimapparlo
            request.Content = new StringContent(JsonConvert.SerializeObject(tipoLezione), Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteOneTipologiaLezioneAsync(int idCliente, int idLocation, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipolezioni/{idLocation}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }


        #region Tipologie Abbonamenti
        public async Task<IEnumerable<TipologieAbbonamentiViewModel>> GetTipologieAbbonamentiClienteAsync(int idCliente, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipoabbonamenti");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<TipologieAbbonamentiViewModel>>(responseString, _serializerSettings);
            return result;
        }

        public async Task<TipologieAbbonamentiViewModel> GetOneTipologiaAbbonamentoAsync(int idCliente, int idTipoAbbonamento, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipoabbonamenti/{idTipoAbbonamento}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<TipologieAbbonamentiViewModel>(responseString, _serializerSettings);
            return result;
        }


        public async Task SaveTipologiaAbbonamentoAsync(int idCliente, TipologieAbbonamentiViewModel tipoAbbonamento, string access_token)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipoabbonamenti");
            HttpRequestMessage request;
            if (tipoAbbonamento.Id.HasValue && tipoAbbonamento.Id.Value > 0)
            {
                request = new HttpRequestMessage(HttpMethod.Put, uri);
            }
            else
            {
                request = new HttpRequestMessage(HttpMethod.Post, uri);
            }
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            //Sfruttiamo il fatto che i tipi di dati sono identici tra API e WEB per evitare di rimapparlo
            request.Content = new StringContent(JsonConvert.SerializeObject(tipoAbbonamento), Encoding.UTF8, "application/json");
            var response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteOneTipologiaAbbonamentoAsync(int idCliente, int idTipoAbbonamento, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipoabbonamenti/{idTipoAbbonamento}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        #endregion

        public async Task<IEnumerable<ScheduleDetailedApiModel>> GetSchedulesAsync(int idCliente, DateTime start, DateTime end, int? idLocation)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules?sd={start.ToString("yyyyMMddTHHmmss")}&ed={end.ToString("yyyyMMddTHHmmss")}&lid={idLocation}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ScheduleDetailedApiModel>>(responseString, _serializerSettings);
            return result;
        }

        public async Task<ScheduleDetailedApiModel> GetScheduleAsync(int idCliente, int idEvento)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ScheduleDetailedApiModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task SaveSchedule(int idCliente, ScheduleApiModel schedule, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules");
            request.Content = new StringContent(JsonConvert.SerializeObject(schedule), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Method = (schedule.Id.HasValue && schedule.Id.Value > 0) ? HttpMethod.Put : HttpMethod.Post;
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<bool> CheckEmail(string email)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/checkemail?email={email}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return bool.Parse((await response.Content.ReadAsStringAsync()));
        }

        public async Task<bool> CheckUrlRoute(string urlRoute, int? idCliente = null)
        {
            string tmpqs = idCliente.HasValue ? $"&idCliente={idCliente}" : "";
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/checkurl?url={urlRoute}{tmpqs}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri);
            response.EnsureSuccessStatusCode();
            return bool.Parse((await response.Content.ReadAsStringAsync()));
        }

        public async Task<UserConfirmationViewModel> ConfermaAccount(string email, string code)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/confirmation?email={email}&code={code}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(uri, null);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<UserConfirmationViewModel>(responseString);
        }

        public async Task GallerySalvaImmagine(int idCliente, ImmagineViewModel image, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.Content = new StringContent(JsonConvert.SerializeObject(image), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            if (image.Id.HasValue && image.Id.Value > 0)
            {
                request.RequestUri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/gallery/{image.Id}");
                request.Method = HttpMethod.Put;
            }
            else
            {
                request.RequestUri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/gallery");
                request.Method = HttpMethod.Post;
            }
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> DeleteImmagineGalleryAsync(int idCliente, int idImage, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/gallery/{idImage}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            return JsonConvert.DeserializeObject<string>(responseString, _serializerSettings);
        }

        public async Task<Models.AppuntamentoViewModel> GetAppuntamentoForCurrentUserAsync(int idCliente, int idEvento, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/appuntamenti?idEvento={idEvento}");
            if (!string.IsNullOrEmpty(access_token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            request.Method = HttpMethod.Get;
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            //Sfruttiamo l'uguaglianza a livello di serializzazione tra i due modelli
            var result = JsonConvert.DeserializeObject<Models.AppuntamentoViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task<Models.AppuntamentoViewModel> GetAppuntamentiForClienteUserAsync(int idCliente, string userId, string access_token, 
                                                    DateTime? dtInizio=null, DateTime? dtFine = null, int pageNumber = 1, int pageSize = 25)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            var sb = new StringBuilder($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/appuntamenti?page={pageNumber}&pageSize={pageSize}");
            if (dtInizio.HasValue)
            {
                sb.Append($"&dtInizio={dtInizio.Value.ToString("yyyyMMddHHmmss")}");
            }
            if (dtFine.HasValue)
            {
                sb.Append($"&dtFine={dtFine.Value.ToString("yyyyMMddHHmmss")}");
            }
            request.RequestUri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/appuntamenti?page={pageNumber}&pageSize={pageSize}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Method = HttpMethod.Get;
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            //Sfruttiamo l'uguaglianza a livello di serializzazione tra i due modelli
            //TODO: Verificare il tipo ritornato
            var result = JsonConvert.DeserializeObject<Models.AppuntamentoViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task SalvaAppuntamentoForCurrentUser(int idCliente, NuovoAppuntamentoApiModel appuntamento, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/appuntamenti");
            request.Content = new StringContent(JsonConvert.SerializeObject(appuntamento), Encoding.UTF8, "application/json");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Method = HttpMethod.Post;
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        /// <summary>
        /// Invocabile da un Utente per ottenere i propri appuntamenti
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<List<AppuntamentoUserApiModel>> GetAppuntamentiForCurrentUserAsync(string userId, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/{userId}/appuntamenti");
            request.Method = HttpMethod.Get;
            if (access_token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<AppuntamentoUserApiModel>>(responseString, _serializerSettings);
            return result;
        }

        /// <summary>
        /// Invocabile da un Utente per ottenere i propri appuntamenti
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<List<AppuntamentoUserApiModel>> GetAppuntamentiForUserAsync(int idCliente, string userId, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/appuntamenti");
            request.Method = HttpMethod.Get;
            if (access_token != null)
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            }
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<List<AppuntamentoUserApiModel>>(responseString, _serializerSettings);
            return result;
        }

        /// <summary>
        /// Ritorna la lista dei clienti followed dall'utente corrente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<List<ClienteFollowed>> ClientiFollowedByUserAsync(string userId, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/{userId}/clientifollowed");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonConvert.DeserializeObject<List<ClienteFollowedApiModel>>(responseString, _serializerSettings);
            return apiResult?.MapToEnumerableClienteFollowed().ToList();
        }

        
        /// <summary>
        /// Verifica se un utente sia già un Follower di un determinato cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<bool> ClienteIsFollowedByUserAsync(int idCliente, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/follow/{idCliente}");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            var responseString = await response.Content.ReadAsStringAsync();
            var apiResult = JsonConvert.DeserializeObject<bool>(responseString, _serializerSettings);
            return apiResult;
        }

        public async Task ClienteFollowAsync(int idCliente, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/follow");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task ClienteUnFollowAsync(int idCliente, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/unfollow");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }



        public async Task<List<ClienteUtenteViewModel>> GetUtentiCliente(int idCliente, string access_token, bool includeStatus, int page = 1, int pageSize = 2000, string sortBy = "Cognome", bool asc=true )
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users?stato={includeStatus}&page={page}&pageSize={pageSize}&sortby={sortBy}&asc={asc}");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ClienteUtenteApiModel>>(responseString, _serializerSettings);
            return result.MapToClienteUtenteViewModel();
        }

        public async Task<ClienteUtenteApiModel> GetUtenteCliente(int idCliente, string userId, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClienteUtenteApiModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task DeleteAppuntamentoAsync(int idCliente, int idAppuntamento, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/appuntamenti/{idAppuntamento}");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = uri;
            request.Method = HttpMethod.Delete;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        #region NOTIFICHE
        public async Task<List<NotificaViewModel>> GetNotificheForUserAsync(string access_token, int? idCliente = null) {
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

        public async Task EditAbbonamentoClienteAsync(int idCliente, AbbonamentoUtenteApiModel abbonamento, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/abbonamenti");
            var content = new StringContent(JsonConvert.SerializeObject(abbonamento), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = uri;
            request.Method = (!abbonamento.Id.HasValue) ? HttpMethod.Post : HttpMethod.Put;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteAbbonamentoClienteAsync(int idCliente, int idAbbonamento, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/abbonamenti/{idAbbonamento}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<AbbonamentoUtenteApiModel>> GetAbbonamentiForUserAsync(int idCliente, string userId, string access_token, bool includeExpired = false, bool includeDeleted = false)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/abbonamenti?idUtente={userId}&incExp={includeExpired}&incDel={includeDeleted}");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<AbbonamentoUtenteApiModel>>(responseString, _serializerSettings);
            return result;
        }

        public async Task<AbbonamentoUtenteApiModel> GetAbbonamentoAsync(int idCliente, int idAbbonamento, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/abbonamenti/{idAbbonamento}");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<AbbonamentoUtenteApiModel>(responseString, _serializerSettings);
            return result;
        }

        #endregion


        #region CERTIFICATI UTENTI-CLIENTI

        public async Task AddCertificatoUtenteClienteAsync(int idCliente, string userId, ClienteUtenteCertificatoApiModel certificato, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/certificati");
            var content = new StringContent(JsonConvert.SerializeObject(certificato), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage();
            request.RequestUri = uri;
            request.Method = (!certificato.Id.HasValue) ? HttpMethod.Post : HttpMethod.Put;
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            request.Content = content;
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task DeleteCertificatoUtenteClienteAsync(int idCliente, string userId, int idCertificato, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/users/{userId}/certificati/{idCertificato}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
        }

        public async Task<IEnumerable<ClienteUtenteCertificatoApiModel>> GetCertificatiForUserAsync(int idCliente, string userId, string access_token, bool includeExpired = false, bool includeDeleted = false)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/certificati?incExp={includeExpired}&incDel={includeDeleted}");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ClienteUtenteCertificatoApiModel>>(responseString, _serializerSettings);
            return result;
        }

        #endregion
    }
}
