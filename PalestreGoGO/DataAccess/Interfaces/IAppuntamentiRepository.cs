using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IAppuntamentiRepository
    {
        Task<AppuntamentoBaseDM> TakeAppuntamentoAsync(int idCliente, string userId, int idSchedule, int? idAbbonamento, string note, string nominativo, string payloadTimeoutManager);

        Task CancelAppuntamentoAsync(int idCliente, int idAppuntamento);
        Task<string> CancelAppuntamentoDaConfermareAsync(int idCliente, int idAppuntamentoDaConfermare);

        /// <summary>
        /// Ritorna tutti gli appuntamenti per uno schedule
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="idSchedule"></param>
        /// <param name="includiConfermari"></param>
        /// <param name="includiNonConfermati"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        Task<IEnumerable<AppuntamentoBaseDM>> GetAllAppuntamenti(int idCliente, int idSchedule, bool includiConfermati, bool includiNonConfermati, bool includeDeleted);
        Task<AppuntamentoBaseDM> GetAppuntamentoForUserAsync(int idCliente, int idSchedule, string userId, bool includeNonConfermati, bool includeDeleted);
        Task<Appuntamenti> GetAppuntamentoAsync(int idCliente, int idAppuntamento);
        Task<Appuntamenti> GetAppuntamentoDaConfermareAsync(int idCliente, int idAppuntamentoDaConfermare);

        /// <summary>
        /// Ritorna tutti gli appuntamenti di un Utente a prescindere dal Cliente presso cui sono presi.
        /// Invocabile solo dall'utente stesso per vedere i propri appuntamenti
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="dtInizioSchedule"></param>
        /// <param name="dtFineSchedule"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        Task<IEnumerable<UtenteClienteAppuntamentoDM>> GetAppuntamentiUtenteAsync(string userId, int pageNumber = 1, int pageSize = 25,
                                                                                        DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                        string sortBy = "DataOraInizio", bool sortAscending = true);

        /// <summary>
        /// Ritorna tutti gli appuntamenti di un Utente presso uno specifico Cliente, invocabile dal gestore per vedere gli appuntamenti di utente presso la propria struttura.
        /// Tecnicamente invocabile anche dall'utente ma al momento non usata in questo scenario
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="userId"></param>
        /// <param name="pageNumber"></param>
        /// <param name="pageSize"></param>
        /// <param name="dtInizioSchedule"></param>
        /// <param name="dtFineSchedule"></param>
        /// <param name="sortBy"></param>
        /// <param name="sortAscending"></param>
        /// <returns></returns>
        Task<IEnumerable<UtenteClienteAppuntamentoDM>> GetAppuntamentiUtenteAsync(int idCliente, string userId, int pageNumber = 1, int pageSize = 25,
                                                                                        DateTime? dtInizioSchedule = null, DateTime? dtFineSchedule = null,
                                                                                        string sortBy = "DataOraInizio", bool sortAscending = true);
    }
}
