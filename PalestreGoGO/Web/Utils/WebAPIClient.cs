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

        public async Task<ClienteViewModel> GetClienteAsync(int idCliente)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClienteViewModel>(responseString, _serializerSettings);
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

        public async Task<Models.TipologieLezioniViewModel> GetOneTipologiaLezione(int idCliente, int idLocation, string access_token)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/tipolezioni/{idLocation}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Models.TipologieLezioniViewModel>(responseString, _serializerSettings);
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

        public async Task<IEnumerable<ScheduleDetailsViewModel>> GetSchedulesAsync(int idCliente, DateTime start, DateTime end, int? idLocation)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules?sd={start.ToString("yyyyMMddTHHmmss")}&ed={end.ToString("yyyyMMddTHHmmss")}&lid={idLocation}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ScheduleDetailsViewModel>>(responseString, _serializerSettings);
            return result;
        }

        public async Task<ScheduleDetailsViewModel> GetScheduleAsync(int idCliente, int idEvento)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ScheduleDetailsViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task SaveSchedule(int idCliente, ScheduleViewModel schedule, string access_token)
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

        public async Task<List<AppuntamentoUserApiModel>> GetAppuntamentiForCurrentUserAsync(Guid userId, string access_token)
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
        /// Verifica se un utente sia già un Follower di un determinato cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<List<ClienteFollowed>> ClientiFollowedByUserAsync(Guid userId, string access_token)
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



        public async Task<List<ClienteUtenteViewModel>> GetUtentiClienteConAbbonamenti(int idCliente, string access_token)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users");
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, uri);
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            string responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ClienteUtenteWithAbbonamentoApiModel>>(responseString, _serializerSettings);
            return result.MapToClienteUtenteViewModel();
        }

        public async Task EditAbbonamentoCliente(int idCliente, AbbonamentoUtenteApiModel abbonamento, string access_token)
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
    }
}
