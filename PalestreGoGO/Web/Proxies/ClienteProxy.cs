using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Web.Authentication;
using Web.Configuration;

namespace Web.Proxies
{
    public class ClienteProxy : BaseAPIProxy
    {
        public ClienteProxy(IOptions<AppConfig> options, IHttpContextAccessor httpContextAccessor,
                    IDistributedCache distributedCache, IOptions<B2CAuthenticationOptions> authOptions) :
                base(options, httpContextAccessor, distributedCache, authOptions)
        { }

        #region CLIENTI
        public async Task<bool> CheckUrlRoute(string urlRoute, int? idCliente = null)
        {
            string tmpqs = idCliente.HasValue ? $"&idCliente={idCliente}" : "";
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/checkurl?url={urlRoute}{tmpqs}");
            return await GetRequestAsync<bool>(uri, false);
        }

        public async Task<string> NuovoClienteAsync(NuovoClienteAPIModel cliente)
        {
            return await SendPostRequestAsync<NuovoClienteAPIModel, string>($"{_appConfig.WebAPI.BaseAddress}api/clienti", cliente);
        }

        public async Task<ClienteDM> GetClienteAsync(int idCliente)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}";
            return await GetRequestAsync<ClienteDM>(new Uri(uri), false);
        }

        public async Task<ClienteDM> GetClienteAsync(string urlRoute)
        {
            Log.Verbose($"Inizio GetClienteAsync({urlRoute})");
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{urlRoute}";
            return await GetRequestAsync<ClienteDM>(new Uri(uri), false);
        }

        public async Task ClienteSalvaProfilo(int idCliente, ClienteProfiloAPIModel profilo)
        {
            await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo", profilo, true);
        }

        public async Task ClienteSalvaBanner(int idCliente, ImmagineClienteInputDM banner)
        {
            if (banner.IdTipoImmagine != (int)TipoImmagineDM.Sfondo) { throw new ArgumentException(nameof(banner)); }
            if (!banner.Id.HasValue) { throw new ArgumentException(nameof(banner)); }
            await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images/{banner.Id}", banner, true);
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

        public async Task GalleryChangeOrder(int idCliente, int[] newOrder)
        {
            await SendPutRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images/order", newOrder, true);
        }

        public async Task<ImmagineClienteDM> DeleteImmagineGalleryAsync(int idCliente, int idImage)
        {
            //Facciamo una DELETE custom perchè ci serve l'url ritornato (eccezionalmente) per poter cancellare il file dallo Storage
            string access_token = await GetAccessTokenAsync();
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images/{idImage}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            using (HttpResponseMessage response = await sClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ImmagineClienteDM>(responseString);
            }
        }

        public async Task<ImmagineClienteDM> GetImmagineClienteAsync(int idCliente, int idImmagine, bool sendAuthToken = true)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images/{idImmagine}");
            return await GetRequestAsync<ImmagineClienteDM>(uri, sendAuthToken);
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

        public async Task<bool> ClienteSalvaAnagrafica(int idCliente, ClienteAnagraficaDM anagrafica)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo/anagrafica";
            HttpStatusCode code =  await SendPutRequestAsync(uri, anagrafica);
            if (code == HttpStatusCode.Redirect) return true;
            else return false;
        }

        #endregion

    }
}
