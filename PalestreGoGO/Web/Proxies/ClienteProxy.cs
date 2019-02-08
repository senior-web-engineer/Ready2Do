﻿using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog;
using ready2do.model.common;
using System.Net.Http;
using System.Net.Http.Headers;
using Newtonsoft.Json;

namespace Web.Proxies
{
    public class ClienteProxy: BaseAPIProxy
    {
        #region CLIENTI
        public async Task<bool> CheckUrlRoute(string urlRoute, int? idCliente = null)
        {
            string tmpqs = idCliente.HasValue ? $"&idCliente={idCliente}" : "";
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/checkurl?url={urlRoute}{tmpqs}");
            return await GetRequestAsync<bool>(uri, false);
        }

        public async Task<bool> NuovoClienteAsync(NuovoClienteAPIModel cliente)
        {
            try
            {
                await SendPostRequestAsync($"{_appConfig.WebAPI.BaseAddress}api/clienti", cliente, false);
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
            return await GetRequestAsync<ClienteDM>(new Uri(uri), false);
        }

        public async Task<ClienteDM> GetClienteAsync(string urlRoute)
        {
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
            HttpRequestMessage request = new HttpRequestMessage(HttpMethod.Delete, $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/images/{idImage}");
            request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", access_token);
            using (HttpResponseMessage response = await sClient.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var responseString = await response.Content.ReadAsStringAsync();
                return JsonConvert.DeserializeObject<ImmagineClienteDM>(responseString);
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

        public async Task ClienteSalvaAnagrafica(int idCliente, AnagraficaClienteApiModel anagrafica)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/profilo/anagrafica";
            await SendPutRequestAsync(uri, anagrafica);
        }

        #endregion

    }
}
