using Common.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.Options;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Authentication;
using Web.Configuration;

namespace Web.Proxies
{
    /// <summary>
    /// Proxy per le API di gestione degli Schedule e degli Appuntamenti
    /// </summary>
    public class SchedulesProxy: BaseAPIProxy
    {
        public SchedulesProxy(IOptions<AppConfig> options, IHttpContextAccessor httpContextAccessor,
            IDistributedCache distributedCache, IOptions<B2CAuthenticationOptions> authOptions) :
        base(options, httpContextAccessor, distributedCache, authOptions)
        { }


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
        public async Task<IEnumerable<AppuntamentoDM>> GetAppuntamentiForEventoAsync(int idCliente, int idEvento)
        {
            var uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}/appuntamenti");
            return await GetRequestAsync<IEnumerable<AppuntamentoDM>>(uri);
        }

        public async Task<IEnumerable<AppuntamentoDaConfermareDM>> GetAppuntamentiDaConferamareForEventoAsync(int idCliente, int idEvento)
        {
            var uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}/appuntamenti/unconfirmed");
            return await GetRequestAsync<IEnumerable<AppuntamentoDaConfermareDM>>(uri);
        }
        public async Task<IEnumerable<WaitListRegistrationDM>> GetWaitListRegistrationsForEventoAsync(int idCliente, int idEvento)
        {
            var uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/schedules/{idEvento}/appuntamenti/waitlist");
            return await GetRequestAsync<IEnumerable<WaitListRegistrationDM>>(uri);
        }


        public async Task TakeAppuntamentoForCurrentUser(int idCliente, NuovoAppuntamentoApiModel appuntamento)
        {
            string uri = $"{_appConfig.WebAPI.BaseAddress}api/clienti/{idCliente}/appuntamenti";
            await SendPostRequestAsync(uri, appuntamento);
        }

        /// <summary>
        /// Invocabile da un Utente per ottenere i propri appuntamenti
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="access_token"></param>
        /// <returns></returns>
        public async Task<List<AppuntamentoUserApiModel>> GetAppuntamentiForCurrentUserAsync(string userId)
        {
            Uri uri = new Uri($"{_appConfig.WebAPI.BaseAddress}api/utenti/{userId}/appuntamenti");
            return await GetRequestAsync<List<AppuntamentoUserApiModel>>(uri);
        }


        #endregion 

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
    }
}
