using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{

    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/users")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientiUtentiController : PalestreControllerBase
    {
        private readonly ILogger<ClientiUtentiController> _logger;
        private readonly IUsersManagementService _userManagementService;
        private readonly IClientiRepository _clientiRepo;
        private readonly IClientiUtentiRepository _clientiUtentiRepo;
        private readonly IAppuntamentiRepository _appuntamentiRepo;

        public ClientiUtentiController(IUsersManagementService userManagementService,
                                        ILogger<ClientiUtentiController> logger,
                                        IClientiRepository clientiRepo,
                                        IClientiUtentiRepository clientiUtentiRepo,
                                        IAppuntamentiRepository appuntamentiRepo)
        {
            _logger = logger;
            _clientiRepo = clientiRepo;
            _userManagementService = userManagementService;
            _clientiUtentiRepo = clientiUtentiRepo;
            _appuntamentiRepo = appuntamentiRepo;
        }

        #region Utenti Clienti
        /// <summary>
        /// Ritorna la lista di utenti associati al Cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> GetClientiUtenti([FromRoute] int idCliente,
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
            IEnumerable<UtenteClienteDM> utenti = await _clientiUtentiRepo.GetUtentiCliente(idCliente, includeStato, page, pageSize, sortCol, sortOrder);
            var result = Mapper.Map<IEnumerable<UtenteClienteDM>, IEnumerable<ClienteUtenteApiModel>>(utenti);
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
        public async Task<IActionResult> GetUserDetails([FromRoute] int idCliente, [FromRoute(Name = "userId")]string userId, [FromQuery(Name = "incStato")] bool includeStato = false)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            UtenteClienteDM utente = await _clientiUtentiRepo.GetUtenteCliente(idCliente, userId, includeStato);
            if (utente == null) return BadRequest(); //Se non l'ho trovato i parametri non sono corretit
            //Se ho trovato l'associazione utente-cliente, recupero i dettagli dell'utente
            AzureUser azUser = await _userManagementService.GetUserByIdAsync(userId);
            if (azUser == null)
            {
                _logger.LogError($"Impossibile trovare l'utente [{userId}] associato al cliente [{idCliente}]");
                return NotFound(); //Forse sarebbe più corretto un 500 invece di un 404
            }
            var result = Mapper.Map<UtenteClienteDM, ClienteUtenteApiModel>(utente);
            result.TelephoneNumber = azUser.TelephoneNumber;
            //Se è cambiato qualcosa nei dati dell'utente su B2C, aggiorniamo i dati locali
            if (!result.Nome.Equals(azUser.Nome) ||
                !result.Cognome.Equals(azUser.Cognome) ||
                !result.DisplayName.Equals(azUser.DisplayName))
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
        public async Task<IActionResult> GetAppuntamentiForUser([FromRoute] int idCliente, [FromRoute(Name = "userId")]string userId,
                                                            string dtInizio = null, string dtFine = null, int pageNumber = 1, int pageSize = 25)
        {
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
            //TODO: convertire in ApiModel e ritornare
            return dmData
        }
    }
}
