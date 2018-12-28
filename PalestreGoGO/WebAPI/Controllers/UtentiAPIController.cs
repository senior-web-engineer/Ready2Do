﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/utenti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UtentiAPIController : APIControllerBase
    {
        private readonly ILogger<UtentiAPIController> _logger;
        private readonly IUsersManagementService _userManagementService;
        private readonly IClientiRepository _repository;
        private readonly IUtentiRepository _repositoryUtenti;
        private readonly IAppuntamentiRepository _repositoryAppuntamenti;

        public UtentiAPIController(ILogger<UtentiAPIController> logger,
                                 IUsersManagementService userManagementService,
                                 IClientiRepository repository,
                                 IAppuntamentiRepository repositoryAppuntamenti,
                                 IUtentiRepository repositoryUtenti)
        {
            this._logger = logger;
            this._userManagementService = userManagementService;
            this._repository = repository;
            this._repositoryAppuntamenti = repositoryAppuntamenti;
            this._repositoryUtenti = repositoryUtenti;
    }

        /// <summary>
        /// Verifica se l'email specificata esiste già nel DB o meno
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Ritorna true se l'email non è stta ancora registrata, false se esiste già un utente con l'email specificata</returns>
        [HttpGet("checkemail")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest();
            }
            var user = await _userManagementService.GetUserByMailAsync(email);
            return Ok(user == null);
        }

        /// <summary>
        /// Registrazione di un Nuovo Utente
        /// </summary>
        /// <param name="newCliente"></param>
        /// <returns></returns>
        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> NuovoUtente([FromBody]NuovoUtenteViewModel newUser, [FromQuery(Name = "idref")]int? idStrutturaAffiliata)
        {
            if (newUser == null)
            {
                return new BadRequestResult();
            }
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            var token = Guid.NewGuid().ToString("N");
            var appUser = new AzureUser(newUser.Email, newUser.Password)
            {
                Cognome = newUser.Cognome,
                Nome = newUser.Nome,
                TelephoneNumber = newUser.Telefono,        
                Refereer = idStrutturaAffiliata?.ToString()
            };

            await _userManagementService.RegisterUserAsync(appUser);

            return Ok();
        }


        /// <summary>
        /// Conferma l'email dell'utente.
        /// Se invocato da un CLIENTE ritorna l'URL della struttura
        /// Se invocato da un UTENTE ordinario r
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns>Ritorna l'url della homepage del cliente appena confermatp in caso di esito positivo</returns>
        [HttpPost("confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfermaUtente([FromQuery]string email, [FromQuery]string code)
        {
            _logger.LogTrace($"ConfirmEmail -> Received request for user: [{email ?? "NULL"}], code: [{code ?? "NULL"}]");
            if (email == null || code == null)
            {
                return BadRequest();
            }
            var esitoConfirmation = await _userManagementService.ConfirmUserEmailAsync(email, code);
            if (!esitoConfirmation.Esito)
            {
                _logger.LogWarning($"ConfirmMail -> Failed validation for user: {email} with code: [{code}]");
                return BadRequest();
            }
            var cliente = await _repository.GetByIdUserOwnerAsync(esitoConfirmation.IdUser);
            if (cliente != null)
            {
                esitoConfirmation.IdCliente = cliente.Id;
            }
            //TODO: Ritornare un CreatedAt con l'url del cliente?
            return Ok(esitoConfirmation);
        }

        [HttpGet("{userId}/appuntamenti")]
        public async Task<IActionResult> GetAppuntamentiForUser([FromRoute] string userId, [FromQuery] bool includePast = false)
        {
            //Verifichiamo che lo userId nella route sia coerente con l'utente chiamante
            if (string.IsNullOrWhiteSpace(User.UserId()) || !User.UserId().Equals(userId))
            {
                return Forbid();
            }
            var appuntamenti = await _repositoryAppuntamenti.GetAppuntamentiUtenteAsync(User.UserId(), dtInizioSchedule: DateTime.Now);
            var result = Mapper.Map<IEnumerable<AppuntamentoUserApiModel>>(appuntamenti);
            return Ok(result);
        }

        [HttpGet("{userId:guid}/clientifollowed")]
        public async Task<IActionResult> GetClientiFollowedForUserAsync([FromRoute] Guid userId)
        {
            //Verifichiamo che lo userId nella route sia coerente con l'utente chiamante
            if (string.IsNullOrWhiteSpace(User.UserId()) || !User.UserId().Equals(userId))
            {
                return Forbid();
            }            
            var result = new List<ClienteFollowedApiModel>();
            var clienti = await _repositoryUtenti.GetGlientiFollowedAsync(userId);
            for(int idx = 0; clienti != null &&  idx < clienti.Count; idx++)
            {
                result.Add(new ClienteFollowedApiModel()
                {
                    IdCliente = clienti[idx].IdCliente,
                    Nome = clienti[idx].Nome,
                    DataFollowing = clienti[idx].DataFollowing,
                    RagioneSociale = clienti[idx].RagioneSociale,
                    AbbonamentoValido = clienti[idx].AbbonamentoValido
                });
            }
            return Ok(result);
        }

        [HttpGet("follow/{idCliente:int}")]
        public async Task<IActionResult> UserFollowCliente([FromRoute] int idCliente)
        {
            string userId = User.UserId();
            //Verifichiamo che lo userId nella route sia coerente con l'utente chiamante
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }
            //Recupero i dati dell'utente da B2C
            var userInfo = await _userManagementService.GetUserByIdAsync(userId);
            bool result = await _repositoryUtenti.UserFollowClienteAsync(userId, idCliente);
            return Ok(result);
        }
    }
}

