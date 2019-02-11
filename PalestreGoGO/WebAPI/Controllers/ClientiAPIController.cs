using Microsoft.ApplicationInsights;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Model.Mappers;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using System.Transactions;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ClientiAPIController : APIControllerBase
    {
        private const int DEFAULT_VALIDATION_VALIDITY = 2880; //48 Ore se non configurato diversamente
        private readonly ILogger<ClientiAPIController> _logger;
        private readonly IClientiRepository _repository;
        private readonly IUsersManagementService _userManagementService;
        private readonly IConfiguration _config;
        private readonly IClientiUtentiRepository _repoClientiUtenti;
        private readonly IClientiProvisioner _clientiProvisioner;
        private readonly IUserConfirmationService _userConfirmationService;
        private readonly IImmaginiClientiRepository _immaginiRepository;
        private readonly TelemetryClient _insightsClient;
        public ClientiAPIController(IConfiguration config,
                                 ILogger<ClientiAPIController> logger,
                                 IUsersManagementService userManagementService,
                                 IClientiRepository repository,
                                 IClientiUtentiRepository repoClientiUtenti,
                                 IClientiProvisioner clientiProvisioner,
                                 IUserConfirmationService userConfirmationService,
                                 IImmaginiClientiRepository immaginiRepository,
                                 TelemetryClient insightsClient)
        {
            _config = config;
            _logger = logger;
            _repository = repository;
            _userManagementService = userManagementService;
            _repoClientiUtenti = repoClientiUtenti;
            _clientiProvisioner = clientiProvisioner;
            _userConfirmationService = userConfirmationService;
            _immaginiRepository = immaginiRepository;
            _insightsClient = insightsClient;
        }

        [HttpGet("{id:int}")]
        [ProducesResponseType(200)]
        [ProducesResponseType(404)]
        [AllowAnonymous]
        public async Task<ActionResult<ClienteDM>> GetCliente([FromRoute(Name = "id")] int idCliente)
        {
            _logger.LogDebug($"Begin [{ControllerContext.HttpContext.Request.Path}]");
            var cliente = await _repository.GetClienteByIdAsync(idCliente);
            if (cliente == null)
            {
                _logger.LogTrace($"Cliente Not Founded: {idCliente}");
                return NotFound();
            }
            return Ok(cliente);
        }

        [HttpGet("{urlroute}")]
        [AllowAnonymous]
        public async Task<ActionResult<ClienteDM>> GetCliente([FromRoute(Name = "urlroute")] string urlRoute)
        {
            _logger.LogDebug($"Begin [{ControllerContext.HttpContext.Request.Path}]");
            if (string.IsNullOrWhiteSpace(urlRoute)) return BadRequest();
            var cliente = await _repository.GetClienteByUrlRouteAsync(urlRoute);
            if (cliente == null)
            {
                _logger.LogDebug($"Cliente Not Founded: {urlRoute}");
                return NotFound();
            }
            return Ok(cliente);
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
        }


        /// <summary>
        /// Registrazione di un Nuovo Cliente associato all'utente chiamante
        /// </summary>
        /// <param name="newCliente"></param>
        /// <returns></returns>
        [HttpPost()]
        public async Task<IActionResult> NuovoCliente([FromBody]NuovoClienteAPIModel newCliente)
        {
            if (newCliente == null) { return new BadRequestResult(); }
            if (!ModelState.IsValid) { return new BadRequestResult(); }
            (int idCliente, Guid correlationId) resultDB = default((int, Guid));
            //2. Creiamo il Cliente sul DB
            //3. Facciamo il Provisioning
            //4. Aggiorniamo l'utente B2C con l'Id del Cliente
            //5. Aggiorniamo lo stato del Cliente (Provisioned)
            //6. Generiamo la richiesta di registrazione
            //7. Inviamo l'email per la conferma
            try
            {
                // Creazione record Cliente sul DB
                Log.Debug("Ricevuta richiesta di registrazione per un nuovo cliente {@newCliente} da perte dell'utente {@userId}", newCliente, GetCurrentUser());
                var userId = GetCurrentUser().UserId();
                var azUser = await _userManagementService.GetUserByIdAsync(userId);
                var nuovoClienteDM = newCliente.ToDM(userId);
                nuovoClienteDM.StorageContainer = Guid.NewGuid().ToString("N").ToLowerInvariant();
                try
                {
                    resultDB = await _repository.CreateClienteAsync(nuovoClienteDM);
                    //Step3: Provisioning Cliente
                    await _clientiProvisioner.ProvisionClienteAsync(resultDB.idCliente, userId);
                    //Step4: Update B2C con la struttura gestita
                    await _userManagementService.AggiungiStrutturaGestitaAsync(azUser, resultDB.idCliente);
                    //Step5: Aggiorniamo lo stato di provisioning del Cliente
                    await _repository.ConfermaProvisioningAsync(resultDB.idCliente);
                }
                catch (Exception exc)
                {
                    Log.Error(exc, "Errore durante la creazione o il provisioning del Cliente. {newCliente}", newCliente);
                    //Se il Cliente è stato creato sul DB lo eliminiamo (Compensation)
                    if (resultDB.idCliente > 0)
                    {
                        try
                        {
                            await _repository.CompensateCreateClienteAsync(resultDB.idCliente, resultDB.correlationId);
                            //Step4: Proviamo a rimuovere da B2C la struttura se c'è stato un errore (ammesso che siamo riusciti a salvarla)
                            await _userManagementService.TryDeleteStrutturaGestitaAsync(userId, resultDB.idCliente);

                        }
                        catch (Exception compExc)
                        {
                            Log.Error(compExc, "Errore durante la compensazione della registrazione Cliente.");
                            throw;
                        }
                    }
                    throw; //Risolleviamo l'eccezione originale così da non eseguire le altre operazioni
                }
                //Step6: Generiamo la richiesta di registrazione
                string code = await _repository.RichiestaRegistrazioneCreaAsync(
                                azUser.Emails.First(),
                                resultDB.correlationId,
                                DateTime.Now.AddMinutes(_config.GetValue<int>("Provisioning:ValidationEmailValidityMinutes", DEFAULT_VALIDATION_VALIDITY)));
                //Step7: Inviamo l'email per la conferma dell'utente
                var email = new Model.ConfirmationMailMessage(azUser.Emails.First(), code, 
                                                              _config.GetValue<string>("Provisioning:EmailConfirmationUrl"),
                                                              true);
                await _userConfirmationService.EnqueueConfirmationMailRequestAsync(email);

                Log.Information("Creato nuovo Cliente con Id: {idCliente} per l'utente {userId}", resultDB.idCliente, userId);
                _insightsClient.TrackEvent("Registrazione_Cliente", new Dictionary<string, string>
                                            {{"IdCliente",resultDB.idCliente.ToString()}, {"UserName",userId }});
            }
            catch (Exception exc)
            {
                Log.Error(exc, "Errore durante la creazione dell'utente {@newCliente}", newCliente);
                _insightsClient.TrackException(exc);
                return this.StatusCode((int)HttpStatusCode.InternalServerError);
            }
            return Ok();
        }

        //[HttpPut("{idCliente:int}/profilo/banner")]
        //public async Task<IActionResult> ClienteSalvaBannerProfilo([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ImmagineClienteInputDM banner)
        //{
        //    if (banner == null) { return BadRequest(); }
        //    if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
        //    if (!ModelState.IsValid)
        //    {
        //        return BadRequest();
        //    }
        //    var existing = (await _immaginiRepository.GetImages(idCliente, TipoImmagineDM.Sfondo)).SingleOrDefault();
        //    if (existing == null)
        //    {
        //        banner.IdTipoImmagine = (int)TipoImmagineDM.Sfondo;
        //        await _immaginiRepository.AddImageAsync(idCliente, banner);
        //    }
        //    else
        //    {
        //        banner.Id = existing.Id;
        //        await _immaginiRepository.UpdateImageAsync(idCliente, banner);
        //    }
        //    return Ok();
        //}

        [HttpPut("{idCliente:int}/profilo/anagrafica")]
        [ProducesResponseType(204)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ClienteAnagraficaDM>> ClienteSalvaAnagrafica([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ClienteAnagraficaDM anagrafica)
        {
            if (anagrafica == null) { return BadRequest(); }
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            if (anagrafica.Id != idCliente) { return BadRequest(); }
            await _repository.AggiornaAnagraficaClienteAsync(idCliente, anagrafica);
            return NoContent();
        }

        [HttpPut("{idCliente:int}/profilo/orario")]
        public async Task<IActionResult> ClienteSalvaOrarioApertura([FromRoute(Name = "idCliente")]int idCliente, [FromBody] OrarioAperturaDM orario)
        {
            if (orario == null) { return BadRequest(); }
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            await _repository.AggiornaOrarioAperturaClienteAsync(idCliente, orario);
            return Ok();
        }


        [HttpPut("{idCliente:int}/profilo")]
        public async Task<IActionResult> ClienteSalvaProfilo([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ClienteProfiloAPIModel profilo)
        {
            if (profilo == null) { return BadRequest(); }
            if (idCliente != profilo.Anagrafica.Id) { return BadRequest(); }
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            //Salviamo le modifiche in transazione
            using (var transaction = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled))
            {
                await _repository.AggiornaAnagraficaClienteAsync(idCliente, profilo.Anagrafica);
                await _repository.AggiornaOrarioAperturaClienteAsync(idCliente, profilo.OrarioApertura);
                await _immaginiRepository.UpdateImageAsync(idCliente, profilo.ImmagineHome);
                transaction.Complete();
            }
            return NoContent();
        }


        //[HttpPost("{idCliente:int}/gallery")]
        //public async Task<IActionResult> ClienteAddImmagineGallery([FromRoute(Name = "idCliente")]int idCliente, [FromBody] ImmagineClienteInputDM immagine)
        //{
        //    if (immagine == null) { return BadRequest(); }
        //    if (immagine.Id.HasValue) { return BadRequest(); } //Non deve avere un Id essendo una nuova immagine
        //    if (!User.CanManageStructure(idCliente)) { return Forbid(); }
        //    if (!ModelState.IsValid) { return BadRequest(); }
        //    var images = new ClientiImmagini[]{
        //        new ClientiImmagini()
        //        {
        //            IdCliente = idCliente,
        //            IdTipoImmagine = Constants.TIPOIMMAGINE_GALLERY,
        //            Alt = immagine.Alt,
        //            Descrizione = immagine.Descrizione,
        //            Nome = immagine.Nome,
        //            Ordinamento = immagine.Ordinamento,
        //            Url = immagine.Url
        //        }
        //    };
        //    await _repository.AddImagesAsync(idCliente, images);
        //    return Ok();
        //}

        //[HttpPut("{idCliente:int}/gallery/{idImage}")]
        //public async Task<IActionResult> ClienteUpdateImmagineGallery([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name = "idImage")] int idImage, [FromBody] ImmagineViewModel immagine)
        //{
        //    if (immagine == null) { return BadRequest(); }
        //    if (!immagine.Id.HasValue || !immagine.Id.Value.Equals(idImage)) { return BadRequest(); } //Deve avere un Id essendo una immagine esistente
        //    if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
        //    if (!ModelState.IsValid) { return BadRequest(); }
        //    var existing = _repository.GetImage(idCliente, idImage);
        //    existing.Alt = immagine.Alt;
        //    existing.Descrizione = immagine.Descrizione;
        //    existing.Nome = immagine.Nome;
        //    existing.Ordinamento = immagine.Ordinamento;
        //    existing.Url = immagine.Url;
        //    await _repository.UpdateImageAsync(idCliente, existing);
        //    return Ok();
        //}

        //[HttpDelete("{idCliente:int}/gallery/{idImage}")]
        //public async Task<IActionResult> ClienteDeleteImmagineGallery([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name = "idImage")] int idImage)
        //{
        //    if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
        //    if (!ModelState.IsValid) { return BadRequest(); }
        //    var existing = _repository.GetImage(idCliente, idImage);
        //    if (existing == null) return BadRequest();
        //    await _repository.DeleteImageAsync(idCliente, idImage);
        //    return Ok(existing.Url);
        //}

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