using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Model.Mappers;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPI.ViewModel.Mappers;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientiAPIController : PalestreControllerBase
    {
        const int DEFAULT_VALIDATION_VALIDITY = 2880; //48 Ore se non configurato diversamente
        private readonly ILogger<ClientiAPIController> _logger;
        private readonly IClientiRepository _repository;
        private readonly IUsersManagementService _userManagementService;
        private readonly IConfiguration _config;
        private readonly IClientiUtentiRepository _repoClientiUtenti;
        private readonly IClientiProvisioner _clientiProvisioner;
        private readonly IUserConfirmationService _userConfirmationService;
        private readonly IImmaginiClientiRepository _immaginiRepository;
        public ClientiAPIController(IConfiguration config,
                                 ILogger<ClientiAPIController> logger,
                                 IUsersManagementService userManagementService,
                                 IClientiRepository repository,
                                 IClientiUtentiRepository repoClientiUtenti,
                                 IClientiProvisioner clientiProvisioner,
                                 IUserConfirmationService userConfirmationService)
        {
            _config = config;
            _logger = logger;
            _repository = repository;
            _userManagementService = userManagementService;
            _repoClientiUtenti = repoClientiUtenti;
            _clientiProvisioner = clientiProvisioner;
            _userConfirmationService = userConfirmationService;
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
            var cliente = await _repository.GetClienteByUrlRouteAsync(urlRoute);
            if (cliente == null) return NotFound();
            var result = Mapper.Map<ClienteWithImagesViewModel>(cliente);
            if (!string.IsNullOrWhiteSpace(cliente.OrarioApertura))
            {
                result.OrarioApertura = JsonConvert.DeserializeObject<OrarioAperturaViewModel>(cliente.OrarioApertura);
            }
            return Ok(result);
        }
      

        [HttpGet("checkurl")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckUrlRoute(string url, int? idCliente = null)
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

            var result = await _repository.CheckUrlRouteValidity(url, idCliente);
            return Ok(result == UrlValidationResultDM.OK);
            //var cliente = await _repository.GetByUrlAsync(url);
            //return Ok(cliente == null);
        }


        /// <summary>
        /// Registrazione di un Nuovo Cliente DA CONFERMARE e senza creazione Utente associato
        /// </summary>
        /// <param name="newCliente"></param>
        /// <returns></returns>
        [HttpPost()]
        [AllowAnonymous]
        public async Task<IActionResult> NuovoCliente([FromBody]NuovoClienteAPIModel newCliente)
        {
            if (newCliente == null) { return new BadRequestResult(); }
            if (!ModelState.IsValid) { return new BadRequestResult(); }

            //1. Creiamo l'utente su B2C (o recuperiamo l'utente già esistente)
            //2. Creiamo il Cliente sul DB
            //3. Facciamo il Provisioning
            //4. Aggiorniamo l'utente B2C con l'Id del Cliente
            //5. Aggiorniamo lo stato del Cliente (Provisioned)
            //6. Generiamo la richiesta di registrazione
            //7. Inviamo l'email per la conferma

            //Step1: Creazione o recupero utente B2C
            var newUser = new AzureUser(newCliente.NuovoUtente.Email, newCliente.NuovoUtente.Password)
            {
                Nome = newCliente.NuovoUtente.Nome,
                Cognome = newCliente.NuovoUtente.Cognome,
                TelephoneNumber = newCliente.NuovoUtente.Telefono,
                DisplayName = newCliente.NuovoUtente.DisplayName ?? $"{newCliente.NuovoUtente.Cognome} {newCliente.NuovoUtente.Nome}"
            };
            var user = await _userManagementService.GetOrCreateUserAsync(newUser);
            string userName = user.SignInNames.First(sn => sn.Type.Equals("emailAddress", StringComparison.InvariantCultureIgnoreCase)).Value;

            //Step2: Creazione record Cliente sul DB
            var nuovoClienteDM = newCliente.ToDM(user.Id);
            nuovoClienteDM.StorageContainer = Guid.NewGuid().ToString("N").ToLowerInvariant();
            var resultDB = await _repository.CreateClienteAsync(nuovoClienteDM);
            //Step3: Provisioning Cliente
            await _clientiProvisioner.ProvisionClienteAsync(resultDB.idCliente, user.Id); 
            //Step4: Update B2C con la struttura gestita
            await _userManagementService.AggiungiStrutturaGestitaAsync(user, resultDB.idCliente);
            //Step5: Aggiorniamo lo stato di provisioning del Cliente
            await _repository.ConfermaProvisioningAsync(resultDB.idCliente);
            //Step6: Generiamo la richiesta di registrazione
            string code = await _repository.RichiestaRegistrazioneCreaAsync(
                            userName,
                            resultDB.correlationId,
                            DateTime.Now.AddMinutes(_config.GetValue<int>("Provisioning:ValidationEmailValidityMinutes", DEFAULT_VALIDATION_VALIDITY)));
            //Step7: Inviamo l'email per la conferma dell'utente
            var email = new Model.ConfirmationMailMessage(newCliente.NuovoUtente.Email, code, true);
            await _userConfirmationService.EnqueueConfirmationMailRequestAsync(email);

            return Ok();
        }

        [HttpPut("{idCliente:int}/profilo/banner")]
        public async Task<IActionResult> ClienteSalvaBannerProfilo([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ImmagineViewModel banner)
        {
            if (banner == null) { return BadRequest(); }
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var existing = (await _immaginiRepository.GetImages(idCliente, TipoImmagineDM.Sfondo)).SingleOrDefault();
            if(existing == null)
            {
                _immaginiRepository.AddImagesAsync(idCliente)
            }
            var img = existing.ClientiImmagini.SingleOrDefault(i => i.IdTipoImmagine == (int)TipoImmagine.Sfondo) ?? new ClientiImmagini()
            {
                IdCliente = idCliente,
                IdTipoImmagine = (int)TipoImmagine.Sfondo,
                Ordinamento = 0,
            };
            img.Url = banner.Url;
            await _repository.UpdateImageAsync(idCliente, img);
            return Ok();
        }

        [HttpPut("{idCliente:int}/profilo/anagrafica")]
        public async Task<IActionResult> ClienteSalvaAnagrafica([FromRoute(Name = "idCliente")]int idCliente, [FromBody] AnagraficaClienteApiModel anagrafica)
        {
            if (anagrafica == null) { return BadRequest(); }
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (anagrafica.IdCliente != idCliente) { return BadRequest(); }


            await _repository.UpdateAnagraficaAsync(anagrafica.ToDM());
            return Ok();
        }

        [HttpPut("{idCliente:int}/profilo/orario")]
        public async Task<IActionResult> ClienteSalvaOrarioApertura([FromRoute(Name = "idCliente")]int idCliente, [FromBody] OrarioAperturaViewModel orario)
        {
            if (orario == null) { return BadRequest(); }
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            await _repository.UpdateOrarioAperturaAsync(idCliente, JsonConvert.SerializeObject(orario));
            return Ok();
        }


        [HttpPut("{idCliente:int}/profilo")]
        public async Task<IActionResult> ClienteSalvaProfilo([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ClienteProfiloViewModel profilo)
        {
            if (profilo == null) { return BadRequest(); }
            if (idCliente != profilo.IdCliente) { return BadRequest(); }
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var existing = await _repository.GetAsync(profilo.IdCliente);

            existing.Descrizione = profilo.Descrizione;
            existing.Citta = profilo.Indirizzo.Citta;
            existing.Latitudine = profilo.Indirizzo.Coordinate.Latitudine;
            existing.Longitudine = profilo.Indirizzo.Coordinate.Longitudine;
            existing.Country = profilo.Indirizzo.Country;
            existing.Indirizzo = profilo.Indirizzo.Indirizzo;
            existing.Nome = profilo.Nome;
            existing.NumTelefono = profilo.NumTelefono;
            existing.RagioneSociale = profilo.RagioneSociale;
            existing.ZipOrPostalCode = profilo.Indirizzo.PostalCode;
            existing.OrarioApertura = JsonConvert.SerializeObject(profilo.OrarioApertura);

            //Se è una nuova immagine, sovrascriviamo la precedente (manteniamo l'Id della vecchia)
            if (!profilo.ImmagineHome.Id.HasValue || profilo.ImmagineHome.Id.Value <= 0)
            {
                var oldImg = existing.ClientiImmagini.SingleOrDefault(i => i.IdTipoImmagine == (int)TipoImmagine.Sfondo) ?? new ClientiImmagini();
                oldImg.IdCliente = idCliente;
                oldImg.IdTipoImmagine = (int)TipoImmagine.Sfondo;
                //oldImg.Nome = profilo.ImmagineHome.Nome;
                //oldImg.Alt = profilo.ImmagineHome.Alt;
                //oldImg.Descrizione = profilo.ImmagineHome.Descrizione;
                oldImg.Ordinamento = 0;
                oldImg.Url = profilo.ImmagineHome.Url;
                await _repository.UpdateImageAsync(idCliente, oldImg);
            }
            await _repository.UpdateAsync(existing);
            return Ok();
        }


        [HttpPost("{idCliente:int}/gallery")]
        public async Task<IActionResult> ClienteAddImmagineGallery([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ImmagineViewModel immagine)
        {
            if (immagine == null) { return BadRequest(); }
            if (immagine.Id.HasValue) { return BadRequest(); } //Non deve avere un Id essendo una nuova immagine
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            var images = new ClientiImmagini[]{
                new ClientiImmagini()
                {
                    IdCliente = idCliente,
                    IdTipoImmagine = Constants.TIPOIMMAGINE_GALLERY,
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
        public async Task<IActionResult> ClienteUpdateImmagineGallery([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name = "idImage")] int idImage, [FromBody] ImmagineViewModel immagine)
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

        ///// <summary>
        ///// 
        ///// </summary>
        ///// <param name="email"></param>
        ///// <param name="code"></param>
        ///// <returns>Ritorna l'url della homepage del cliente appena confermatp in caso di esito positivo</returns>
        //[HttpPost("confirmation")]
        //[AllowAnonymous]
        //public async Task<IActionResult> ConfermaCliente([FromQuery]string email, [FromQuery]string code)
        //{
        //    _logger.LogTrace($"ConfirmEmail -> Received request for user: [{email ?? "NULL"}], code: [{code ?? "NULL"}]");
        //    if (email == null || code == null)
        //    {
        //        return BadRequest();
        //    }
        //    var esitoConfirmation = await _userManagementService.ConfirmUserAsync(email, code);
        //    if (!esitoConfirmation.Esito)
        //    {
        //        _logger.LogWarning($"ConfirmMail -> Failed validation for user: {email} with code: [{code}]");
        //        return BadRequest();
        //    }
        //    var cliente = await _repository.GetByIdUserOwner(esitoConfirmation.IdUser);
        //    esitoConfirmation.IdCliente = cliente.Id;
        //    //TODO: Ritornare un CreatedAt con l'url del cliente?
        //    return Ok(esitoConfirmation);
        //}

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
            var userId = GetCurrentUser().UserId();
            if (string.IsNullOrWhiteSpace(userId)) return Forbid();    //Se non ho trovato l'utente ritorniamo 403 - Forbidden
            var userInfo = await _userManagementService.GetUserByIdAsync(userId);
            await _repoClientiUtenti.AssociaUtenteAsync(idCliente, userId, userInfo.Nome, userInfo.Cognome, userInfo.DisplayName);
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
            var userId = GetCurrentUser().UserId();
            if (!string.IsNullOrWhiteSpace(userId)) return Forbid();    //Se non ho trovato l'utente ritorniamo 403 - Forbidden
            await _repoClientiUtenti.DisassociaUtenteFollowerAsync(idCliente, userId);
            return Ok();
        }

    }
}