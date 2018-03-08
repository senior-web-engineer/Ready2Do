using Newtonsoft.Json;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Web.Models;

namespace Web.Utils
{
    public static class WebAPIClient
    {
        private readonly static JsonSerializerSettings _serializerSettings;

        static WebAPIClient()
        {
            _serializerSettings = new JsonSerializerSettings();
        }

        public async static Task<bool> NuovoClienteAsync(NuovoClienteViewModel cliente, string baseUrl)
        {
            Uri uri = new Uri($"{baseUrl}api/clienti");
            var content = new StringContent(JsonConvert.SerializeObject(cliente), Encoding.UTF8, "application/json");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.PostAsync(uri, content);
            return response.IsSuccessStatusCode;
        }

        public async static Task<ClienteViewModel> GetClienteAsync(int idCliente, string baseUrl)
        {        
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}api/clienti/{idCliente}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClienteViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async static Task<ClienteViewModel> GetClienteAsync(string urlRoute, string baseUrl)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}api/clienti/{urlRoute}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClienteViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async static Task<ClienteViewModel> GetClienteFromTokenAsync(string securityToken, string baseUrl)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}api/clienti/token/{securityToken}");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ClienteViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async static Task<IEnumerable<TipologiaClienteViewModel>> GetTipologiClientiAsync(string baseUrl)
        {
            Uri uri = new Uri($"{baseUrl}api/clienti/tipologie");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            //NOTA: Forziamo la deserializzazione al tipo "locale" e non quello originale nell'assemblu WebApiDataModel
            var result = JsonConvert.DeserializeObject<IEnumerable<TipologiaClienteViewModel>>(responseString, _serializerSettings);
            return result;
        }

        public async static Task<IEnumerable<Models.LocationViewModel>> GetLocationsAsync(int idCliente, string baseUrl)
        {
            HttpClient client = new HttpClient();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Get, $"{baseUrl}api/{idCliente}/tipologiche/locations");
            //request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            HttpResponseMessage response = await client.SendAsync(request);
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<Models.LocationViewModel>>(responseString, _serializerSettings);
            return result;
        }


        public async static Task<IEnumerable<Models.TipologieLezioniViewModel>> GetTipologieLezioniClienteAsync(string baseUrl, int idCliente)
        {
            Uri uri = new Uri($"{baseUrl}api/{idCliente}/tipologiche/tipolezioni");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            //NOTA: Forziamo la deserializzazione al tipo "locale" e non quello originale nell'assembly WebApiDataModel
            var result = JsonConvert.DeserializeObject<IEnumerable<Models.TipologieLezioniViewModel>>(responseString, _serializerSettings);
            return result;
        }
        public async static Task<IEnumerable<ScheduleDetailsViewModel>> GetSchedulesAsync(string baseUrl, int idCliente, DateTime start, DateTime end, int? idLocation)
        {
            Uri uri = new Uri($"{baseUrl}api/clienti/{idCliente}/schedules?sd={start.ToString("yyyyMMddTHHmmss")}&ed={end.ToString("yyyyMMddTHHmmss")}&lid={idLocation}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<IEnumerable<ScheduleDetailsViewModel>>(responseString, _serializerSettings);
            return result;
        }

        public async static Task<ScheduleDetailsViewModel> GetScheduleAsync(string baseUrl, int idCliente, int idEvento)
        {
            Uri uri = new Uri($"{baseUrl}api/clienti/{idCliente}/schedules/{idEvento}");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            var result = JsonConvert.DeserializeObject<ScheduleDetailsViewModel>(responseString, _serializerSettings);
            return result;
        }

        public async static Task SaveSchedule(int idCliente, string baseUrl, ScheduleViewModel schedule)
        {
            Uri uri = new Uri($"{baseUrl}api/clienti/{idCliente}/schedules");
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
    }
}
