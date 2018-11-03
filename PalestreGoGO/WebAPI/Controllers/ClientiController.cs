﻿using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Services;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPIModel;
using System;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using PalestreGoGo.WebAPI.ViewModel.B2CGraph;
using PalestreGoGo.WebAPI.ViewModel.Mappers;

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
        private readonly IConfiguration _config;

        public ClientiController(IConfiguration config, 
                                 ILogger<ClientiController> logger,
                                 IUsersManagementService userManagementService,
                                 IClientiRepository repository)
        {
            _config = config;
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


        ///// <summary>
        ///// Registrazione di un Nuovo Cliente CONTESTUALMENTE ad un nuovo Utente
        ///// </summary>
        ///// <param name="newCliente"></param>
        ///// <returns></returns>
        //[HttpPost()]
        //[AllowAnonymous]
        //public async Task<IActionResult> NuovoCliente([FromBody]NuovoClienteViewModel newCliente)
        //{
        //    if (newCliente == null)
        //    {
        //        return new BadRequestResult();
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return new BadRequestResult();
        //    }
        //    var token = Guid.NewGuid().ToString("N");
        //    // Step 1 - Salviamo i dati del Cliente
        //    var cliente = new DataModel.Clienti()
        //    {
        //        Citta = newCliente.Citta,
        //        Latitudine = newCliente.Coordinate.Latitudine,
        //        Longitudine = newCliente.Coordinate.Longitudine,
        //        Country = newCliente.Country,
        //        DataProvisioning = DateTime.Now,
        //        Email = newCliente.Email,
        //        IdTipologia = newCliente.IdTipologia,
        //        Indirizzo = newCliente.Indirizzo,
        //        Nome = newCliente.Nome,
        //        NumTelefono = newCliente.NumTelefono,
        //        SecurityToken = token,
        //        RagioneSociale = newCliente.RagioneSociale,
        //        ZipOrPostalCode = newCliente.ZipOrPostalCode,
        //        StorageContainer = token,
        //        UrlRoute = newCliente.UrlRoute
        //    };
        //    //Aggiungiamo la Hero Image di default 
        //    cliente.ClientiImmagini.Add(new ClientiImmagini()
        //    {
        //        IdTipoImmagine = (int)TipoImmagine.Sfondo,
        //        Url = _config.GetValue<string>("Provisioning:DefaultHeroImageUrl"),
        //        Nome = "Default Hero Image",
        //        IdCliente = cliente.Id
        //    });
        //    await _repository.AddAsync(cliente);

        //    //Step 2 - Creiamo l'utente Owner
        //    var user = new AppUser()
        //    {
        //        UserName = newCliente.NuovoUtente.Email,
        //        FirstName = newCliente.NuovoUtente.Nome,
        //        LastName = newCliente.NuovoUtente.Cognome,
        //        Email = newCliente.NuovoUtente.Email,
        //        PhoneNumber = newCliente.NuovoUtente.Telefono,
        //        CreationToken = token
        //    };
        //    await _userManagementService.RegisterOwnerAsync(user, newCliente.NuovoUtente.Password, cliente.Id.ToString());

        //    return Ok();
        //}

        /// <summary>
        /// Registrazione di un Nuovo Cliente DA CONFERMARE e senza creazione Utente associato
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
            var correlationId = Guid.NewGuid();
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
                SecurityToken = correlationId.ToString("N"),
                RagioneSociale = newCliente.RagioneSociale,
                ZipOrPostalCode = newCliente.ZipOrPostalCode,
                StorageContainer = correlationId.ToString("N"),
                UrlRoute = newCliente.UrlRoute
            };
            //Aggiungiamo la Hero Image di default 
            cliente.ClientiImmagini.Add(new ClientiImmagini()
            {
                IdTipoImmagine = (int)TipoImmagine.Sfondo,
                Url = _config.GetValue<string>("Provisioning:DefaultHeroImageUrl"),
                Nome = "Default Hero Image",
                IdCliente = cliente.Id
            });
            await _repository.AddAsync(cliente);

            //Step 2 - Creiamo l'utente Owner
            var user = new LocalAccountUser(newCliente.NuovoUtente.Email, newCliente.NuovoUtente.Password)
            {                
                Nome = newCliente.NuovoUtente.Nome,
                Cognome = newCliente.NuovoUtente.Cognome,                
                TelephoneNumber = newCliente.NuovoUtente.Telefono
            };
            await _userManagementService.RegisterOwnerAsync(user, cliente.Id.ToString(), correlationId);

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
            var existing = await _repository.GetAsync(idCliente);

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
            if(anagrafica.IdCliente != idCliente) { return BadRequest(); }


            await _repository.UpdateAnagraficaAsync(anagrafica.ToDM());
            return Ok();
        }

        [HttpPut("{idCliente:int}/profilo/orario")]
        public async Task<IActionResult> ClienteSalvaOrarioApertura([FromRoute(Name = "idCliente")]int idCliente, [FromBody] OrarioAperturaViewModel orario)
        {
            if (orario == null) { return BadRequest(); }
            if (!User.CanManageStructure(idCliente)) { return Unauthorized(); }
            if (!ModelState.IsValid){return BadRequest();}            
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
            if(!profilo.ImmagineHome.Id.HasValue || profilo.ImmagineHome.Id.Value <= 0)
            {
                var oldImg = existing.ClientiImmagini.SingleOrDefault(i=>i.IdTipoImmagine == (int)TipoImmagine.Sfondo) ?? new ClientiImmagini();
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
            if (!ModelState.IsValid){return BadRequest();}
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