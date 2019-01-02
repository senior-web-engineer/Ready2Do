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
        private readonly ISchedulesRepository _schedulesRepo;

        public ClientiUtentiAPIController(IUsersManagementService userManagementService,
                                        ILogger<ClientiUtentiAPIController> logger,
                                        IClientiRepository clientiRepo,
                                        IClientiUtentiRepository clientiUtentiRepo,
                                        IAppuntamentiRepository appuntamentiRepo,
                                        ISchedulesRepository schedulesRepo)
        {
            _logger = logger;
            _clientiRepo = clientiRepo;
            _userManagementService = userManagementService;
            _clientiUtentiRepo = clientiUtentiRepo;
            _appuntamentiRepo = appuntamentiRepo;
            _schedulesRepo = schedulesRepo;
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
        /// Ritorna il dettaglio di un utente associato ad un cliente
        /// </summary>
        /// <remarks>
        /// Può essere invocata solo dal gestore del tenant
        /// </remarks>
        /// <param name="idCliente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{userId}")]
        public async Task<ActionResult<ClienteUtenteDetailsApiModel>> GetUserDetails([FromRoute] int idCliente, [FromRoute(Name = "userId")]string userId, [FromQuery(Name = "incStato")] bool includeStato = false)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            UtenteClienteDM utente = await _clientiUtentiRepo.GetUtenteCliente(idCliente, userId, includeStato);
            if (utente == null) return BadRequest(); //Se non l'ho trovato i parametri non sono corretti
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
        #endregion

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
                    Schedule = schedules.Single(s=>s.Id.Equals(a.ScheduleId))
                });
            }
            return Ok(result);
        }
    }
}
