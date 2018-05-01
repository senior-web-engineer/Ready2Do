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

        public async Task SaveLocationAsync(int idCliente, Models.LocationViewModel location)
        {
            HttpClient client = new HttpClient();
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations");
            //Sfruttiamo il fatto che i tipi di dati sono identici tra API e WEB per evitare di rimapparlo
            var content = new StringContent(JsonConvert.SerializeObject(location), Encoding.UTF8, "application/json");
            HttpResponseMessage response;
            if (location.Id.HasValue && location.Id.Value > 0)
            {
                response = await client.PutAsync(uri, content);
            }
            else
            {
                response = await client.PostAsync(uri, content);
            }
            response.EnsureSuccessStatusCode();            
        }

        public async Task<Models.LocationViewModel> GetOneLocationAsync(int idCliente, int idLocation)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{idLocation}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<Models.LocationViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async Task DeleteOneLocationAsync(int idCliente, int idLocation)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/{idCliente}/tipologiche/locations/{idLocation}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
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

        public async Task SaveSchedule(int idCliente, ScheduleViewModel schedule)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules");
            var content = new StringContent(JsonConvert.SerializeObject(schedule), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpResponseMessage response;
            if (schedule.Id.HasValue && schedule.Id.Value > 0)
            {
                response = await client.PutAsync(uri, content);
            }
            else
            {
                response = await client.PostAsync(uri, content);
            }
            response.EnsureSuccessStatusCode();
        }

        public async Task<string> ConfermaAccount(string email, string code)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/confirmation?email={email}&code={code}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(uri, null);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadAsStringAsync();
        }

        public async Task GallerySalvaImmagine(int idCliente, ImmagineViewModel image, string token)
        {
            Uri uri;
            var content = new StringContent(JsonConvert.SerializeObject(image), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            client.SetBearerToken(token);
            HttpResponseMessage response;
            if (image.Id.HasValue && image.Id.Value > 0)
            {
                uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/gallery/{image.Id}");
                response = await client.PutAsync(uri, content);
            }
            else
            {
                uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/gallery");
                response = await client.PostAsync(uri, content);
            }
            response.EnsureSuccessStatusCode();
        }

        //public async Task GallerySalvaImmagine(int idCliente, ImmagineViewModel image, string token)
        //{
        //    Uri uri;
        //    var content = new StringContent(JsonConvert.SerializeObject(image), Encoding.UTF8, "application/json");
        //    HttpClient client = new HttpClient();
        //    client.SetBearerToken(token);
        //    HttpResponseMessage response;
        //    if (image.Id.HasValue && image.Id.Value > 0)
        //    {
        //        uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/gallery/{image.Id}");
        //        response = await client.PutAsync(uri, content);
        //    }
        //    else
        //    {
        //        uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/gallery");
        //        response = await client.PostAsync(uri, content);
        //    }
        //    response.EnsureSuccessStatusCode();
        //}
    }
}
