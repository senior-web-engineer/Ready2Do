﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Models;
using Web.Configuration;
using Newtonsoft.Json;
using System.Text;
using Web.Models.Utils;
using System.Globalization;
using PalestreGoGo.WebAPIModel;
using Microsoft.AspNetCore.Mvc.Rendering;
using System.Security.Claims;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Web.Services;
using System.Net;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class ClientiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;
        private readonly ClienteResolverServices _clientiResolver;

        public ClientiController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 WebAPIClient apiClient,
                                 ClienteResolverServices clientiResolver
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
            _clientiResolver = clientiResolver;
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<IActionResult> CheckUrl([FromQuery(Name ="UrlRoute")]string url,[FromQuery(Name="IdCliente")] int? idCliente)
        {
            if (string.IsNullOrWhiteSpace(url))
            {
                return Json(data: $"Richiesta di validazione non valida.");
            }
            if (!Uri.IsWellFormedUriString(url, UriKind.Relative))
            {
                return Json(data: $"L'URL specificato non è un url valido.");
            }
            if (url.Contains("/"))
            {
                return Json(data: $"Non è possibile specificare più di un segmento.");
            }
            if (url.Contains("?"))
            {
                return Json(data: $"Non è possibile specificare una querystring");
            }
            //Se viene passato un idCliente da escludere dalla validazione deve essere quello gestito dall'utente chiamante
            if (idCliente.HasValue && !User.GetUserTypeForCliente(idCliente.Value).IsAtLeastAdmin())
            {
                return Json(data: $"Richiesta di validazione non valida.");
            }
            bool urlIsValid = false;
            try
            {
                urlIsValid = await _apiClient.CheckUrlRoute(url, idCliente);
            }
            catch(Exception exc)
            {
                _logger.LogError(exc, $"Errore durante la validazione del Url {url}");
                urlIsValid = false;
            }
            if (!urlIsValid)
            {
                return Json(data: $"L'URL specificato risulta già utilizzato. Inserirne un altro.");
            }
            else
            {
                return Json(data: urlIsValid);
            }
        }

        [HttpGet]
        [Route("/{cliente}", Name = "HomeCliente")]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Se non troviamo il cliente redirect alla home
            if (cliente == null) { return Redirect("/"); }
            var locations = await _apiClient.GetLocationsAsync(cliente.IdCliente);

            ViewData["ReturnUrl"] = Request.Path.ToString();
            ViewData["Sale"] = locations;
            ViewData["AuthToken"] = SecurityUtils.GenerateAuthenticationToken(urlRoute, cliente.IdCliente, _appConfig.EncryptKey);
            ViewData["ClienteRoute"] = urlRoute;
            ViewData["MapUrl"] = this.BuildMapUrlForCliente(cliente);
            ViewData["IdCliente"] = cliente.IdCliente;
            ViewBag.UtenteNormale = User.GetUserTypeForCliente(cliente.IdCliente) == UserType.NormalUser;
            ViewBag.IsFollowing = false;
            if (ViewBag.UtenteNormale)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                var follwed = await _apiClient.ClientiFollowedByUserAsync(User.UserId().Value, accessToken);
                if (follwed.Any(f => f.IdCliente.Equals(cliente.IdCliente))){
                    ViewBag.IsFollowing = true;
                }
            }
            //ViewData["UserRole"] = User.GetUserRoleForCliente(cliente.IdCliente);
            return View(cliente.MapToHomeViewModel());
        }

        private object BuildMapUrlForCliente(ClienteWithImagesViewModel cliente)
        {
            return $"https://maps.googleapis.com/maps/api/staticmap?markers=size:mid%7Clabel:{WebUtility.UrlEncode(cliente.Nome)}%7C{cliente.Indirizzo.Coordinate.Latitudine},{cliente.Indirizzo.Coordinate.Longitudine}&size=640x250&scale=2&maptype=roadmap&zoom=14&key={_appConfig.GoogleAPI.GoogleMapsAPIKey}";
        }

        [HttpGet("{cliente}/gallery")]
        public async Task<IActionResult> GalleryEdit([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin()) { return Forbid(); }
            ViewData["SASToken"] = SecurityUtils.GenerateSASAuthenticationToken(cliente.SecurityToken, cliente.StorageContainer, _appConfig.EncryptKey);
            ViewData["IdCliente"] = cliente.IdCliente;
            var vm = new GalleryEditViewModel();
            vm.ContainerUrl = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                                _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                                cliente.StorageContainer);
            if (cliente.Immagini != null)
            {
                foreach (var img in cliente.Immagini)
                {
                    vm.Images.Add(new ImageViewModel()
                    {
                        Id = img.Id,
                        Alt = img.Alt,
                        Caption = img.Nome,
                        Url = img.Url,
                        Ordinamento = img.Ordinamento
                    });
                }
            }
            return View("Gallery", vm);
        }

        [HttpDelete("{cliente}/gallery/delete/{imageId}")]
        public async Task<IActionResult> DeleteImage([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "imageId")]int imageId)
        {
            //var cliente = await _apiClient.GetClienteAsync(urlRoute);
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            var userType = User.GetUserTypeForCliente(idCliente);
            if (!userType.IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var urlImage = await _apiClient.DeleteImmagineGalleryAsync(idCliente, imageId, accessToken);
            await AzureStorageUtils.DeleteBlobAsync(_appConfig.Azure, urlImage);
            return await GalleryEdit(urlRoute);
        }


        [HttpGet("{cliente}/profilo")]
        public async Task<IActionResult> ProfileEdit([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            var userType = User.GetUserTypeForCliente(cliente.IdCliente);
            if (!userType.IsAtLeastAdmin())
            {
                return Forbid();
            }
            var vm = cliente.MapToProfileEditVM();
            ViewData["IdCliente"] = cliente.IdCliente;
            ViewData["SASToken"] = SecurityUtils.GenerateSASAuthenticationToken(cliente.SecurityToken, cliente.StorageContainer, _appConfig.EncryptKey);
            ViewData["ContainerUrl"] = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                                _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                               cliente.StorageContainer);
            ViewData["MapUrl"] = this.BuildMapUrlForCliente(cliente);

            return View("Profilo", vm);
        }

        [HttpPost("{cliente}/profilo")]
        public async Task<IActionResult> ProfileEdit([FromRoute(Name = "cliente")]string urlRoute, ClienteProfileEditViewModel profilo)
        {
            float latitudine, longitudine;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken))
            {
                return Forbid();
            }
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            var userType = User.GetUserTypeForCliente(cliente.IdCliente);
            if (!userType.IsAtLeastAdmin())
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View("Profilo", profilo);
            }
            //NOTA: dato che usando direttamente il tipo float nel ViewModel abbiamo problemi di Culture dobbiamo parsarla a mano
            if (!float.TryParse(profilo.Latitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out latitudine) ||
                !float.TryParse(profilo.Longitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out longitudine))
            {
                ModelState.AddModelError(string.Empty, "Coordinate non valide");
                return View("Profilo", profilo);
            }
            //Salviamo il profilo
            var apiModel = profilo.MapToAPIModel();
            await _apiClient.ClienteSalvaProfilo(cliente.IdCliente, apiModel, accessToken);
            return RedirectToAction("ProfileEdit", new { cliente = urlRoute });
        }

        
        #region Gestione Associazione Utenti

        /// <summary>
        /// Associa l'utente corrente alla struttura (cliente)
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <returns></returns>
        [HttpPost("{cliente}/associa")]
        public async Task<IActionResult> AddAssociazioneUserToCliente([FromRoute(Name = "cliente")]string urlRoute, [FromQuery(Name ="returnUrl")]string returnUrl)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken)) { return Forbid(); }
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _apiClient.ClienteFollowAsync(idCliente, accessToken);
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                //Possibile problema di sicurezza? (Open Redirect?)
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", new { cliente = urlRoute });
            }
        }

        /// <summary>
        /// Associa l'utente corrente alla struttura (cliente)
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <returns></returns>
        [HttpPost("{cliente}/disassocia")]
        public async Task<IActionResult> RemoveAssociazioneUserToCliente([FromRoute(Name = "cliente")]string urlRoute)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken)) { return Forbid(); }
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _apiClient.ClienteUnFollowAsync(idCliente, accessToken);
            return RedirectToAction("Index", new { cliente = urlRoute });
        }
        #endregion

        #region Gestione Utenti del Cliente
        [HttpGet("{cliente}/users")]
        public async Task<IActionResult> GetUtentiCliente([FromRoute(Name = "cliente")]string urlRoute)
        {
            var idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di gestione degli utenti
            var userType = User.GetUserTypeForCliente(idCliente);
            if (!userType.IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var tipologieAbbnamenti = await _apiClient.GetTipologieAbbonamentiClienteAsync(idCliente, accessToken);
            var vm = await _apiClient.GetUtentiClienteConAbbonamenti(idCliente, accessToken);
            ViewBag.IdCliente = idCliente;
            if (tipologieAbbnamenti != null && tipologieAbbnamenti.Count() > 0)
            {
                ViewBag.TipologieAbbonamenti = tipologieAbbnamenti.Select(ta => new KeyValuePair<int, string>(ta.Id.Value, ta.Nome));
            }
            return View("Utenti",vm);
        }
        #endregion

        //#region Helpers
        ///// <summary>
        ///// Genera una stringa rappresentante un "token" per l'autenticazione delle chiamate Ajax
        ///// </summary>
        ///// <param name="secuirtyToken"></param>
        ///// <param name="storageContainer"></param>
        ///// <returns></returns>
        //private string GenerateSASAuthenticationToken(string secuirtyToken, string storageContainer)
        //{
        //    var token = new SASTokenModel()
        //    {
        //        SecurityToken = secuirtyToken,
        //        ContainerName = storageContainer,
        //        CreationTime = DateTime.Now
        //    };
        //    string json = JsonConvert.SerializeObject(token, Formatting.None);
        //    //Cifriamo il json ottenuto
        //    var result = SecurityUtils.EncryptStringWithAes(json, Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
        //    return result;
        //}

        //private string GenerateAuthenticationToken(string clientRouteUrl, int idCliente)
        //{
        //    var token = new AuthTokenModel()
        //    {
        //        ClientRoute = clientRouteUrl,
        //        CreationTime = DateTime.Now,
        //        IdCliente = idCliente
        //    };
        //    string json = JsonConvert.SerializeObject(token, Formatting.None);
        //    //Cifriamo il json ottenuto
        //    var result = SecurityUtils.EncryptStringWithAes(json, Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
        //    return result;
        //}
        //#endregion
    }
}