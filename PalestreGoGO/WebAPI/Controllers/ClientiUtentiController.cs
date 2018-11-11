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
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
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
        /// Ritorna la lista di utenti associati al Cliente
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [HttpGet()]
        public async Task<IActionResult> GetClientiUtenti([FromRoute] int idCliente)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();

            var follower = await _clientiRepo.GetAllFollowersWithAbbonamenti(idCliente);
            var result = Mapper.Map<IEnumerable<ClienteUtenteConAbbonamento>, IEnumerable<ClienteUtenteWithAbbonamentoApiModel>>(follower);
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
        [HttpGet("{id}")]
        public async Task<IActionResult> GetUserDetails([FromRoute] int idCliente, [FromRoute]string id)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            var follower = await _clientiRepo.GetFollowerAsync(idCliente, id);
            if (follower == null) return BadRequest(); //Se non l'ho trovato i parametri non sono corretit
            //Se ho trovato l'associazione utente-cliente, recupero i dettagli dell'utente
            var user = await _userManagementService.GetUserByIdAsync(id.ToString());
            if(user == null)
            {
                _logger.LogError($"Impossibile trovare l'utente [{id}] associato al cliente [{idCliente}]");
                return this.NotFound(); //Forse sarebbe più corretto un 500 invece di un 404
            }
            //var result = Mapper.Map<AppUser, ClienteUtenteApiModel>(user);
            //result.IdCliente = idCliente;
            //result.DataAssociazione = follower.DataCreazione;

            //return Ok(result);
            return Ok();
        }
        #endregion


    }
}
