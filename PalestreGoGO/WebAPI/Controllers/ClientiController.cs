using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.ViewModel;
using System;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti")]
    //[Authorize(Policy ="AdminResources")]
    public class ClientiController : ControllerBase
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

        [HttpGet]
        public IActionResult GetCliente([FromQuery(Name ="id")] int idCliente)
        {
            return null;
        }

        /// <summary>
        /// Registrazione di un Nuovo Cliente CONTESTUALMENTE ad un nuovo Utente
        /// </summary>
        /// <param name="cliente"></param>
        /// <param name="newUser"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> NuovoCliente([FromBody]NuovoClienteViewModel newCliente) {
            if((newCliente == null) || (newCliente == null))
            {
                return new BadRequestResult();
            }
            if (!this.ModelState.IsValid)
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
            await _repository.CommitAsync();

            //Step 2 - Creiamo l'utente Owner
            AppUser user = new AppUser()
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

        //[HttpPut]
        //[Authorize(Policy ="ProvisioningPolicy")]
        //public IActionResult ProvisioningCliente(int idCliente, Guid owner, string token)
        //{
        //    var cliente = _dbContext.Clienti.Single(c => c.Id == idCliente && c.IdUserOwner == owner && c.ProvisioningToken.Equals(token));
        //    if (cliente == null) return BadRequest();
            
        //    throw new NotImplementedException();
        //}
    }
}