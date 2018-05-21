using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.IdentityModel;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCliente([FromRoute(Name = "id")] int idCliente)
        {
            var cliente = await _repository.GetAsync(idCliente);
            var result = Mapper.Map<ClienteWithImagesViewModel>(cliente);
            if (!string.IsNullOrWhiteSpace(cliente.OrarioApertura))
            {
                result.OrarioApertura = JsonConvert.DeserializeObject<OrarioAperturaViewModel>(cliente.OrarioApertura);
            }
            return Ok(result);
        }

        [HttpGet("{urlroute}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetCliente([FromRoute(Name = "urlroute")] string urlRoute)
        {
            var cliente = await _repository.GetByUrlAsync(urlRoute);
            if (cliente == null) return NotFound();
            var result = Mapper.Map<ClienteWithImagesViewModel>(cliente);
            if (!string.IsNullOrWhiteSpace(cliente.OrarioApertura))
            {
                result.OrarioApertura = JsonConvert.DeserializeObject<OrarioAperturaViewModel>(cliente.OrarioApertura);
            }
            return Ok(result);
        }

        [HttpGet("token/{securityToken}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetClienteByToken([FromRoute(Name = "securityToken")] string securityToken)
        {
            var cliente = await _repository.GetByTokenAsync(securityToken);
            var result = Mapper.Map<ClienteWithImagesViewModel>(cliente);
            if (!string.IsNullOrWhiteSpace(cliente.OrarioApertura))
            {
                result.OrarioApertura = JsonConvert.DeserializeObject<OrarioAperturaViewModel>(cliente.OrarioApertura);
            }
            return Ok(result);
        }

        /// <summary>
        /// Verifica se l'url specificato è già registrato
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Ritorna true se l'email non è stta ancora registrata, false se esiste già un utente con l'email specificata</returns>
        [HttpGet("checkurl")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUrlRoute(string url)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return BadRequest();
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                return Ok(false);
            }
            if (url.Contains("/"))
            {
                return Ok(false);
            }
            if (url.Contains("?"))
            {
                return Ok(false);
            }
            var cliente = await _repository.GetByUrlAsync(url);
            return Ok(cliente == null);
        }


        /// <summary>
        /// Registrazione di un Nuovo Cliente CONTESTUALMENTE ad un nuovo Utente
        /// </summary>
        /// <param name="newCliente"></param>
        /// <returns></returns>
        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> NuovoCliente([FromBody]NuovoClienteViewModel newCliente)
        {
            if (newCliente == null)
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
                SecurityToken = token,
                RagioneSociale = newCliente.RagioneSociale,
                ZipOrPostalCode = newCliente.ZipOrPostalCode,
                StorageContainer = token,
                UrlRoute = newCliente.UrlRoute
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

        [HttpPut("{idCliente:int}")]
        public async Task<IActionResult> ClienteSalvaProfilo([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ClienteViewModel cliente)
        {
            if (cliente == null) { return BadRequest(); }
            if (idCliente != cliente.IdCliente) { return BadRequest(); }
            if (!ClaimsPrincipal.Current.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var existing = await _repository.GetAsync(cliente.IdCliente);

            existing.Descrizione = cliente.Descrizione;
            existing.Citta = cliente.Indirizzo.Citta;
            existing.Latitudine = cliente.Indirizzo.Coordinate.Latitudine;
            existing.Longitudine = cliente.Indirizzo.Coordinate.Longitudine;
            existing.Country = cliente.Indirizzo.Country;
            existing.Indirizzo = cliente.Indirizzo.Indirizzo;
            existing.Nome = cliente.Nome;
            existing.NumTelefono = cliente.NumTelefono;
            existing.RagioneSociale = cliente.RagioneSociale;
            existing.ZipOrPostalCode = cliente.Indirizzo.PostalCode;
            await _repository.UpdateAsync(existing);
            return Ok();
        }


        [HttpPost("{idCliente:int}/gallery")]
        public async Task<IActionResult> ClienteAddImmagineGallery([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ImmagineViewModel immagine)
        {
            if (immagine == null) { return BadRequest(); }
            if (immagine.Id.HasValue) { return BadRequest(); } //Non deve avere un Id essendo una nuova immagine
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid){return BadRequest();}
            var images = new ClientiImmagini[]{
                new ClientiImmagini()
                {
                    IdCliente = idCliente,
                    IdTipoImmagine = Constants.TIPOIMMAGINE_SFONDO,
                    Alt = immagine.Alt,
                    Descrizione = immagine.Descrizione,
                    Nome = immagine.Nome,
                    Ordinamento = immagine.Ordinamento,
                    Url = immagine.Url
                }
            };
            await _repository.AddImagesAsync(idCliente, images);
            return Ok();
        }

        [HttpPut("{idCliente:int}/gallery/{idImage}")]
        public async Task<IActionResult> ClienteUpdateImmagineGallery([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name ="idImage")] int idImage, [FromBody] ImmagineViewModel immagine)
        {
            if (immagine == null) { return BadRequest(); }
            if (!immagine.Id.HasValue || !immagine.Id.Value.Equals(idImage)) { return BadRequest(); } //Deve avere un Id essendo una immagine esistente
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            var existing = _repository.GetImage(idCliente, idImage);            
            existing.Alt = immagine.Alt;
            existing.Descrizione = immagine.Descrizione;
            existing.Nome = immagine.Nome;
            existing.Ordinamento = immagine.Ordinamento;
                    existing.Url = immagine.Url;
            await _repository.UpdateImageAsync(idCliente, existing);
            return Ok();
        }

        [HttpDelete("{idCliente:int}/gallery/{idImage}")]
        public async Task<IActionResult> ClienteDeleteImmagineGallery([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name = "idImage")] int idImage)
        {
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            var existing = _repository.GetImage(idCliente, idImage);
            if (existing == null) return BadRequest();
            await _repository.DeleteImageAsync(idCliente, idImage);
            return Ok(existing.Url);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="email"></param>
        /// <param name="code"></param>
        /// <returns>Ritorna l'url della homepage del cliente appena confermatp in caso di esito positivo</returns>
        [HttpPost("confirmation")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfermaCliente([FromQuery]string email, [FromQuery]string code)
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
            var cliente = await _repository.GetByIdUserOwner(esitoConfirmation.IdUser);
            esitoConfirmation.IdCliente = cliente.Id;
            //TODO: Ritornare un CreatedAt con l'url del cliente?
            return Ok(esitoConfirmation);
        }

        //public async Task<IActionResult> AssociaImmagine(ImmagineViewModel immagine)
        //{
        //    return Ok();
        //}

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