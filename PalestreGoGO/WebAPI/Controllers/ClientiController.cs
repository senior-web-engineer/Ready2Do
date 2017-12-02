using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti")]
    [Authorize()]
    public class ClientiController : PalestreControllerBase
    {
        private readonly ILogger<ClientiController> _logger;
        private readonly IClientiRepository _repository;
        private readonly IUsersManagementService _userManagementService;

        public ClientiController(ILogger<ClientiController> logger,
                                 IUsersManagementService userManagementService, 
                                 IClientiRepository repository)
        {
            _logger = logger;
            _repository = repository;
            _userManagementService = userManagementService;
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCliente([FromRoute(Name ="id")] int idCliente)
        {
            var cliente = await _repository.GetAsync(idCliente);

            return Ok(Mapper.Map<ClienteViewModel>(cliente));
        }

        /// <summary>
        /// Registrazione di un Nuovo Cliente CONTESTUALMENTE ad un nuovo Utente
        /// </summary>
        /// <param name="newCliente"></param>
        /// <returns></returns>
        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> NuovoCliente([FromBody]NuovoClienteViewModel newCliente) {
            if(newCliente == null)
            {
                return new BadRequestResult();
            }
            if (!ModelState.IsValid)
            {
                return new BadRequestResult();
            }
            var token = Guid.NewGuid().ToString("N");
            // Step 1 - Salviamo i dati del Cliente
            var cliente = new DataModel.Clienti()
            {
                Citta = newCliente.Citta,
                Latitudine = newCliente.Coordinate.Latitudine,
                Longitudine = newCliente.Coordinate.Longitudine,
                Country = newCliente.Country,
                DataProvisioning = DateTime.Now,
                Email = newCliente.Email,
                IdTipologia = newCliente.IdTipologia,
                Indirizzo = newCliente.Indirizzo,
                Nome = newCliente.Nome,
                NumTelefono = newCliente.NumTelefono,
                ProvisioningToken = token,
                RagioneSociale = newCliente.RagioneSociale,
                ZipOrPostalCode = newCliente.ZipOrPostalCode
            };
            await _repository.AddAsync(cliente);            

            //Step 2 - Creiamo l'utente Owner
            var user = new AppUser()
            {
                UserName = newCliente.NuovoUtente.Email,
                FirstName = newCliente.NuovoUtente.Nome,
                LastName = newCliente.NuovoUtente.Cognome,
                Email = newCliente.NuovoUtente.Email,
                PhoneNumber = newCliente.NuovoUtente.Telefono,
                CreationToken = token
            };
            await _userManagementService.RegisterOwnerAsync(user, newCliente.NuovoUtente.Password, cliente.Id.ToString());

            return Ok();
        }

        [HttpPost("confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfermaCliente([FromQuery]string email, [FromQuery]string code)
        {
            _logger.LogTrace($"ConfirmEmail -> Received request for user: [{email ?? "NULL"}], code: [{code ?? "NULL"}]");
            if (email == null || code == null)
            {
                return BadRequest();
            }
            bool esito = await _userManagementService.ConfirmUserAsync(email, code);
            if (!esito)
            {
                _logger.LogWarning($"ConfirmMail -> Failed validation for user: {email} with code: [{code}]");
                return BadRequest();
            }
            //TODO: Ritornare un CreatedAt con l'url del cliente?
            return new OkResult();
        }

        /// <summary>
        /// Aggiunge l'utente chiamante (estrapolato dal Token) come Follower del cliente (estrapolato dalla route)
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [HttpPost("{idCliente}/follow")]
        public async Task<IActionResult> Follow([FromRoute] int idCliente)
        {
            var userId = this.GetCurrentUser().UserId();
            if (!userId.HasValue) return Forbid();    //Se non ho trovato l'utente ritorniamo 403 - Forbidden
            await _repository.AddUtenteFollowerAsync(idCliente, userId.Value);
            return Ok();
        }

        /// <summary>
        /// Rimuove l'utente chiamante (estrapolato dal Token) come Follower del cliente (estrapolato dalla route)
        /// </summary>
        /// <param name="idCliente"></param>
        /// <returns></returns>
        [HttpPost("{idCliente}/unfollow")]
        public async Task<IActionResult> UnFollow([FromRoute] int idCliente)
        {
            var userId = this.GetCurrentUser().UserId();
            if (!userId.HasValue) return Forbid();    //Se non ho trovato l'utente ritorniamo 403 - Forbidden
            await _repository.RemoveUtenteFollowerAsync(idCliente, userId.Value);
            return Ok();
        }

    }
}