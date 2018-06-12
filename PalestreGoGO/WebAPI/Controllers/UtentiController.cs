using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Services;
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
    public class UtentiController : PalestreControllerBase
    {
        private readonly ILogger<UtentiController> _logger;
        private readonly IUsersManagementService _userManagementService;
        private readonly IClientiRepository _repository;

        public UtentiController(ILogger<UtentiController> logger,
                                 IUsersManagementService userManagementService,
                                 IClientiRepository repository)
        {
            this._logger = logger;
            this._userManagementService = userManagementService;
            this._repository = repository;
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
        public async Task<IActionResult> NuovoUtente([FromBody]NuovoUtenteViewModel newUser, [FromQuery(Name = "idref")]int idStrutturaAffiliata)
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
            var appUser = new AppUser()
            {
                UserName = newUser.Email,
                FirstName = newUser.Nome,
                LastName = newUser.Cognome,
                Email = newUser.Email,
                CreationToken = token
            };
            await _userManagementService.RegisterUserAsync(appUser, newUser.Password, idStrutturaAffiliata);

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
            var esitoConfirmation = await _userManagementService.ConfirmUserAsync(email, code);
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

    }
}

