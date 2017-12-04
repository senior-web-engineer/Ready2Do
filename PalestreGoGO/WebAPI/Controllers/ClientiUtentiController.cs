using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPIModel;
using System;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{

    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/users")]
    [Authorize]
    public class ClientiUtentiController : PalestreControllerBase
    {
        private readonly ILogger<ClientiUtentiController> _logger;
        private readonly IUsersManagementService _userManagementService;
        private readonly IClientiRepository _clientiRepo;

        public ClientiUtentiController( IUsersManagementService userManagementService,
                                        ILogger<ClientiUtentiController> logger,
                                        IClientiRepository clientiRepo)
        {
            this._logger = logger;
            this._clientiRepo = clientiRepo;
            this._userManagementService = userManagementService;
        }

        #region Utenti Clienti
        /// <summary>
        /// Ritorna il dettaglio di un utente associato ad un cliente
        /// </summary>
        /// <remarks>
        /// Può essere invocata solo dal gestore del tenant
        /// </remarks>
        /// <param name="idCliente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] int idCliente, [FromRoute]Guid id)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            var follower = await _clientiRepo.GetFollowerAsync(idCliente, id);
            if (follower == null) return BadRequest(); //Se non l'ho trovato i parametri non sono corretit
            //Se ho trovato l'associazione utente-cliente, recupero i dettagli dell'uente
            var user = await _userManagementService.GetUserByIdAsync(id);
            if(user == null)
            {
                _logger.LogError($"Impossibile trovare l'utente [{id}] associato al cliente [{idCliente}]");
                return this.NotFound(); //Forse sarebbe più corretto un 500 invece di un 404
            }
            var result = Mapper.Map<AppUser, ClienteUtenteViewModel>(user);
            return Ok(result);
        }
        #endregion


    }
}
