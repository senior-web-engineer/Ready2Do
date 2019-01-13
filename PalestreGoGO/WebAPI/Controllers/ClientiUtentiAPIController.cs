using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;
using System.Linq;
using Common.Utils;
using PalestreGoGo.WebAPI.Business;

namespace PalestreGoGo.WebAPI.Controllers
{

    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientiUtentiAPIController : APIControllerBase
    {
        private readonly ILogger<ClientiUtentiAPIController> _logger;
        private readonly IUsersManagementService _userManagementService;
        private readonly IClientiRepository _clientiRepo;
        private readonly IClientiUtentiRepository _clientiUtentiRepo;
        private readonly IAppuntamentiRepository _appuntamentiRepo;
        private readonly IAbbonamentiRepository _abbonamentiRepo;
        private readonly ISchedulesRepository _schedulesRepo;
        private readonly UtentiBusiness _utentiBusiness;

        public ClientiUtentiAPIController(IUsersManagementService userManagementService,
                                        ILogger<ClientiUtentiAPIController> logger,
                                        IClientiRepository clientiRepo,
                                        IClientiUtentiRepository clientiUtentiRepo,
                                        IAppuntamentiRepository appuntamentiRepo,
                                        IAbbonamentiRepository abbonamentiRepo,
                                        ISchedulesRepository schedulesRepo,
                                        UtentiBusiness utentiBusiness)
        {
            _logger = logger;
            _clientiRepo = clientiRepo;
            _userManagementService = userManagementService;
            _clientiUtentiRepo = clientiUtentiRepo;
            _appuntamentiRepo = appuntamentiRepo;
            _schedulesRepo = schedulesRepo;
            _abbonamentiRepo = abbonamentiRepo;
            _utentiBusiness = utentiBusiness;
        }

        #region Utenti Clienti
        /// <summary>
        /// Ritorna la lista di utenti associati al Cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<ActionResult<IEnumerable<UtenteClienteDM>>> GetClientiUtenti([FromRoute] int idCliente,
                                                          [FromQuery(Name = "stato")]bool includeStato = false,
                                                          [FromQuery(Name = "page")]int page = 1,
                                                          [FromQuery(Name = "pageSize")]int pageSize = 25,
                                                          [FromQuery(Name = "sortby")]string sortby = "Cognome",
                                                          [FromQuery(Name = "asc")]bool asc = true)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            ClientiUtentiListaSortColumnDM sortCol;
            if (!Enum.TryParse<ClientiUtentiListaSortColumnDM>(sortby, out sortCol))
            {
                sortCol = ClientiUtentiListaSortColumnDM.Cognome;
            }
            SortOrderDM sortOrder = asc ? SortOrderDM.Ascending : SortOrderDM.Descending;
            IEnumerable<UtenteClienteDM> result = await _clientiUtentiRepo.GetUtentiCliente(idCliente, includeStato, page, pageSize, sortCol, sortOrder);
            return Ok(result);
        }


        /// <summary>
        /// Ritorna il dettaglio di un'associazione di un utente ad un cliente
        /// </summary>
        /// <remarks>
        /// Non ritorna i dati anagrafici aggiornati dell'utente (quelli su B2C) ma la copia locale. 
        /// Utilizzare la GetUserProfile per ottenere i dati aggiornati da B2C (che ha come side effect l'aggiornamento della copia locale).
        /// </remarks>
        /// <param name="idCliente">ID del cliente (dalla route)</param>
        /// <param name="userId">Identificativo dell'utente (dalla route)</param>
        /// <param name="numAbbonamentiToIncl">indica il numero massimo di abbonamenti da includere nella risposta</param>
        /// <param name="includeAppuntamentiFrom">DataOra in formato ISO8601 indicante la data da cui si vogliono gli appuntamenti</param>
        /// <param name="includeAppuntamentiTo">DataOra in formato ISO8601 indicante la data fino a cui si vogliono gli appuntamenti</param>
        /// <param name="includeAppuntamentiDaConfermare">Flag indicante se includere anche gli appuntamenti da confermare nel range di date specificate dagli altri parametri</param>
        /// <param name="includeCertificati">Flag indicante se includere nella risposta anche i Certificati per l'utente</param>
        /// <returns>Ritorna i dettagli dell'utente</returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<AssociazioneUtenteClienteDM>> GetDetagliAssociazioneUtente([FromRoute] int idCliente, [FromRoute(Name = "userId")]string userId,
                                                                                                [FromQuery(Name = "incAbb")] int? numAbbonamentiToIncl = 0,
                                                                                                [FromQuery(Name = "apFrom")] string includeAppuntamentiFrom = null,
                                                                                                [FromQuery(Name = "apTo")] string includeAppuntamentiTo = null,
                                                                                                [FromQuery(Name = "incAppDaConf")] bool? includeAppuntamentiDaConfermare = false,
                                                                                                [FromQuery(Name = "incCert")] bool? includeCertificati = false)
        {
            //Solo i gestori della struttura o gli utenti (solo per se stessi) possono richiamare l'operazione
            if (!GetCurrentUser().CanManageStructure(idCliente) || !GetCurrentUser().UserId().Equals(userId)) return Forbid();
            DateTime? appuntamentiFrom = includeAppuntamentiFrom.FromIS8601();
            if (!string.IsNullOrEmpty(includeAppuntamentiFrom) && !appuntamentiFrom.HasValue) { return BadRequest(); /* formato data errato*/ }
            DateTime? appuntamentiTo = includeAppuntamentiTo.FromIS8601();
            if (!string.IsNullOrEmpty(includeAppuntamentiTo) && !appuntamentiTo.HasValue) { return BadRequest(); /* formato data errato*/ }

            AssociazioneUtenteClienteDM result = await _utentiBusiness.GetDettagliAssociazionoConCliente(idCliente, userId, numAbbonamentiToIncl, appuntamentiFrom, appuntamentiTo,
                                                                                                         includeAppuntamentiDaConfermare, includeCertificati);
            return Ok(result);
        }

        /// <summary>
        /// Ritorna le informazioni anagrafiche di un utente aggiornate da B2C integrandole con i dati sull'associazione con il Cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("{userId}/profile")]
        public async Task<ActionResult<ClienteUtenteDetailsApiModel>> GetUserProfile([FromRoute] int idCliente, [FromRoute(Name = "userId")]string userId)
        {
            //Solo i gestori della struttura possono richiamare l'operazione
            if (!GetCurrentUser().CanManageStructure(idCliente)) { return Forbid(); }
            var utente = await _clientiUtentiRepo.GetUtenteCliente(idCliente, userId, false);
            if (utente == null) { return BadRequest(); }

            //Se ho trovato l'associazione utente-cliente, recupero i dettagli dell'utente
            AzureUser azUser = await _userManagementService.GetUserByIdAsync(userId);
            if (azUser == null)
            {
                _logger.LogError($"Impossibile trovare l'utente [{userId}] associato al cliente [{idCliente}]");
                return NotFound(); //Forse sarebbe più corretto un 500 invece di un 404
            }
            var result = new ClienteUtenteDetailsApiModel()
            {
                Nome = azUser.Nome,
                DisplayName = azUser.DisplayName,
                Email = azUser.Emails.FirstOrDefault(),
                TelephoneNumber = azUser.TelephoneNumber,
                Cognome = azUser.Cognome,

                IdCliente = utente.IdCliente,
                IdUtente = azUser.Id,
                DataAssociazione = utente.DataAssociazione.Value,
                DataCancellazione = utente.DataCancellazione,
                Stato = utente.Stato
            };

            //Se è cambiato qualcosa nei dati dell'utente su B2C, aggiorniamo i dati locali
            if (!utente.Nome.Equals(azUser.Nome) ||
                !utente.Cognome.Equals(azUser.Cognome) ||
                !utente.DisplayName.Equals(azUser.DisplayName))
            {
                await _clientiUtentiRepo.AssociaUtenteAsync(idCliente, userId, azUser.Nome, azUser.Cognome, azUser.DisplayName);
            }
            return Ok(result);

        }


        /// <summary>
        /// Invocabile dall'owner di una struttura per ottenere tutti gli appuntamenti per un utente
        /// </summary>
        [HttpGet("{userId}/appuntamenti")]
        public async Task<ActionResult<IEnumerable<AppuntamentoUserApiModel>>> GetAppuntamentiForUser([FromRoute] int idCliente, [FromRoute(Name = "userId")]string userId,
                                                            string dtInizio = null, string dtFine = null, int pageNumber = 1, int pageSize = 25)
        {
            IEnumerable<AppuntamentoUserApiModel> result = null;
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            DateTime? dataInizio = null, dataFine = null;
            DateTime appo;
            if (string.IsNullOrWhiteSpace(dtInizio) && DateTime.TryParseExact(dtInizio, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out appo))
            {
                dataInizio = appo;
            }
            if (string.IsNullOrWhiteSpace(dtFine) && DateTime.TryParseExact(dtFine, "yyyyMMddHHmmss", CultureInfo.InvariantCulture, DateTimeStyles.None, out appo))
            {
                dataFine = appo;
            }
            var dmData = await _appuntamentiRepo.GetAppuntamentiUtenteAsync(idCliente, userId, pageNumber, pageSize, dataInizio, dataFine);
            if (dmData != null)
            {
                var idSchedules = dmData.Select(a => a.ScheduleId).Distinct();
                var schedules = await _schedulesRepo.SchedulesLookupAsync(idSchedules, true);
                result = dmData.Select(a => new AppuntamentoUserApiModel()
                {
                    Appuntamento = a,
                    Schedule = schedules.Single(s => s.Id.Equals(a.ScheduleId))
                });
            }
            return Ok(result);
        }

        #endregion

    }
}
