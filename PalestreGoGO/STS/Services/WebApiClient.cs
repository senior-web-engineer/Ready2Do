using Newtonsoft.Json;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Palestregogo.STS.Services
{
    public static class WebAPIClient
    {
        private readonly static JsonSerializerSettings _serializerSettings;

        static WebAPIClient()
        {
            _serializerSettings = new JsonSerializerSettings();
        }

        public async static Task<IEnumerable<Palestregogo.STS.UI.Model.TipologiaClienteViewModel>> GetTipologiClientiAsync(string baseUrl)
        {
            Uri uri = new Uri($"{baseUrl}api/clienti/tipologie");
            HttpClient client = new HttpClient();
            HttpResponseMessage response = await client.GetAsync(uri); ;
            response.EnsureSuccessStatusCode();
            String responseString = await response.Content.ReadAsStringAsync();
            //NOTA: Forziamo la deserializzazione al tipo "locale" e non quello originale nell'assemblu WebApiDataModel
            var result = JsonConvert.DeserializeObject<IEnumerable<Palestregogo.STS.UI.Model.TipologiaClienteViewModel>>(responseString, _serializerSettings);
            return result;
        }

        public async static Task<bool> NuovoClienteAsync(NuovoClienteViewModel cliente, string baseUrl)
        {
            Uri uri = new Uri($"{baseUrl}api/clienti");
            //HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Post, $"{baseUrl}api/clienti");
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
    }
}
