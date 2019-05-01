using Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Web.Authentication;
using Web.Configuration;
using Web.Models;
using Web.Models.Mappers;

namespace Web.Proxies
{
    /// <summary>
    /// Proxy per le API relative agli Utenti ed alla loro gestione
    /// </summary>
    public class UtentiProxy : BaseAPIProxy
    {
        public UtentiProxy(IOptions<AppConfig> options, IHttpContextAccessor httpContextAccessor,
                        IDistributedCache distributedCache, IOptions<B2CAuthenticationOptions> authOptions) :
                    base(options, httpContextAccessor, distributedCache, authOptions)
        { }

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

        //public async Task<bool> NuovoUtenteAsync(NuovoUtenteViewModel utente, int? idStrutturaAffiliata)
        //{
        //    //TODO: Rimuovere l'idStrutturaAffiliata dalla querystring e metterlo nel body
        //    Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti?idref={idStrutturaAffiliata}");
        //    var content = new StringContent(JsonConvert.SerializeObject(utente), Encoding.UTF8, "application/json");
        //    HttpClient client = new HttpClient();
        //    HttpResponseMessage response = await client.PostAsync(uri, content);
        //    return response.IsSuccessStatusCode;
        //}

        public async Task<UserConfirmationResultAPIModel> ConfermaAccount(string email, string code)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/confirmation?email={email}&code={code}");
            using (var response = await SendRequestAsync<UserConfirmationResultAPIModel>(HttpMethod.Post, uri, null, false))
            {
                return JsonConvert.DeserializeObject<UserConfirmationResultAPIModel>(await response.Content.ReadAsStringAsync());
            }
        }

        public async Task<bool> IsAccountConfirmedAsync (string email){
            return await GetRequestAsync<bool>(new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/isconfirmed/{email}"), true);
        }

        public async Task SendNewConfirmEmail(string email)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/utenti/send-confirm-email/{WebUtility.UrlEncode(email)}";
            await SendPostRequestAsync<string>(uri, null, true);
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

        /// <summary>
        /// Questo metodo viene chiamato nella validazione del Cookie, prima che il Principal corrente sia valorizzato con i Claim (che devono essere ancora letti dal cookie)
        /// pertanto non possiamo usare il metodo della classe base per recuperare il Token, dobbiamo farci passare 
        /// </summary>
        /// <returns></returns>
        public async Task<AzureUser> GetMyIdPProfileAsync(string userId, string authnclassreference)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/me/ip");
            var token = await GetAccessTokenAsync(userId, authnclassreference);
            return await GetRequestAsync<AzureUser>(uri, token);
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
        public async Task<bool> ClienteIsFollowedByUserAsync(int idCliente)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/follow/{idCliente}");
            return await GetRequestAsync<bool>(uri);
        }

        public async Task ClienteFollowAsync(int idCliente)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/follow";
            await SendPostRequestAsync(uri, "");
        }

        public async Task ClienteUnFollowAsync(int idCliente)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/unfollow";
            await SendPostRequestAsync(uri, "");
        }

        public async Task<List<ClienteUtenteViewModel>> GetUtentiCliente(int idCliente, string access_token, bool includeStatus, int page = 1, int pageSize = 2000, string sortBy = "Cognome", bool asc = true)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users?stato={includeStatus}&page={page}&pageSize={pageSize}&sortby={sortBy}&asc={asc}");
            if (!includeStatus)
            {
                var clienti = await GetRequestAsync<IEnumerable<UtenteClienteDM>>(uri, true);
                return clienti.MapToClienteUtenteViewModel();
            }
            else
            {
                var clienti = await GetRequestAsync<IEnumerable<UtenteClienteDetailsDM>>(uri, true);
                return clienti.MapToClienteUtenteViewModel();
            }
        }

        public async Task<AssociazioneUtenteClienteDM> GetUtenteCliente(int idCliente, string userId, DateTime? appuntamentiFrom = null, DateTime? appuntamentiTo = null, bool? includeAppuntamentiDaConfermare = false)
        {
            bool hasQS = false;
            StringBuilder queryString = new StringBuilder();
            if (appuntamentiFrom.HasValue)
            {
                if (!hasQS)
                {
                    queryString.Append($"?apFrom={appuntamentiFrom.Value.ToISO8601(true)}");
                    hasQS = true;
                }
                else
                {
                    queryString.Append($"?apFrom={appuntamentiFrom.Value.ToISO8601(true)}");
                }
            }
            if (appuntamentiTo.HasValue)
            {
                if (!hasQS)
                {
                    queryString.Append($"?apTo={appuntamentiTo.Value.ToISO8601(true)}");
                    hasQS = true;
                }
                else
                {
                    queryString.Append($"&apTo={appuntamentiTo.Value.ToISO8601(true)}");
                }
            }
            if (includeAppuntamentiDaConfermare.HasValue)
            {
                if (!hasQS)
                {
                    queryString.Append($"?incAppDaConf={includeAppuntamentiDaConfermare.Value}");
                    hasQS = true;
                }
                else
                {
                    queryString.Append($"&incAppDaConf={includeAppuntamentiDaConfermare.Value}");
                }
            }

            
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}{queryString.ToString()}");

            return await GetRequestAsync<AssociazioneUtenteClienteDM>(uri);
        }

        /// <summary>
        /// Invocabile da un Utente per ottenere i propri appuntamenti
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<List<AppuntamentoUserApiModel>> GetAppuntamentiForUserAsync(int idCliente, string userId)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/appuntamenti");
            return await GetRequestAsync<List<AppuntamentoUserApiModel>>(uri);
        }

        /// <summary>
        /// Invocabile solo dal gestore per confermare tutti gli appuntamenti non confermati per un utente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        public async Task ConfermaAppuntamentiUtenteAsync(int idCliente, string userId)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/users/{userId}/appuntamenti/conferma";
            await SendPostRequestEmptyAsync(uri, true);
        }



        #region NOTIFICHE
        public async Task<List<NotificaConTipoApiModel>> GetNotificheForUserAsync(int? idCliente = null)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/users/notifiche");
            return await GetRequestAsync<List<NotificaConTipoApiModel>>(uri);
        }
        #endregion

    }
}
