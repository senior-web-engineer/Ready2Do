using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ready2do.model.common;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Authentication;
using Web.Configuration;
using Web.Filters;
using Web.Models;
using Web.Models.Mappers;
using Web.Proxies;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [ServiceFilter(typeof(ReauthenticationRequiredFilter))]
    public class ClientiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly ClienteProxy _clienteProxy;
        private readonly TipologicheProxy _tipologicheProxy;
        private readonly ClienteResolverServices _clientiResolver;

        public ClientiController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 ClienteProxy clienteProxy,
                                 TipologicheProxy tipologicheProxy,
                                 ClienteResolverServices clientiResolver
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _clienteProxy = clienteProxy;
            _clientiResolver = clientiResolver;
            _tipologicheProxy = tipologicheProxy;
        }

        /// <summary>
        /// L'action è configurata come AllowAnonymous (anche se in realtà è accessibile soloa gli utenti autenticati) per gestire
        /// esplicitamente il redirect all'action di Signup
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("/register")]
        public async Task<IActionResult> Registrazione()
        {
            //Se l'utente non è Autenticato ==> lo facciamo autenticare usando l'action specifica che valorizza lo State opportunamente
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignupCliente", "Account");
            }

            ViewBag.TipologieClienti = await this.GetTipologieClientiAsync();
            ViewBag.GoogleMapsAPIKey = _appConfig?.GoogleAPI?.GoogleMapsAPIKey;
            var vm = new ClienteRegistrazioneViewModel();
            return View("Registrazione", vm);
        }

        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SalvaRegistrazione(ClienteRegistrazioneViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.TipologieClienti = await this.GetTipologieClientiAsync();
                ViewBag.GoogleMapsAPIKey = _appConfig?.GoogleAPI?.GoogleMapsAPIKey;
                return View("Registrazione", model);
            }
            try
            {
                await _clienteProxy.NuovoClienteAsync(model.MapToAPIModel());
                //Se l'utente che ha registrato la nuova struttura non ha ancora confermato l'email lo rimandiamo alla pagina di conferma email
                if (!User.EmailConfirmedOn().HasValue)
                {
                    return RedirectToAction("MailToConfirm", "Account");
                }
            }
            catch (ReauthenticationRequiredException)
            {
                Log.Debug("ReauthenticationRequiredException detected");
                throw;
            }
            catch (Exception)
            {
                Log.Error("Errore durante la registrazione del cliente {@cliente}", model);
                ModelState.AddModelError(string.Empty, "Si è verificato un errore durante la registrazione, si prega di riprovare più tardi");
                return View("Registrazione", model);

            }
            return Ok();
        }


        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<IActionResult> CheckUrl([FromQuery(Name = "UrlRoute")]string url, [FromQuery(Name = "IdCliente")] int? idCliente)
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
                urlIsValid = await _clienteProxy.CheckUrlRoute(url, idCliente);
            }
            catch (Exception exc)
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
        [Route("/{idCliente:int}")]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromRoute(Name = "idCliente")]int idCliente)
        {
            var cliente = await _clienteProxy.GetClienteAsync(idCliente);
            //Se non troviamo il cliente redirect alla home
            if (cliente == null) { return Redirect("/"); }
            //Leggiamo le immagini per il cliente
            var imgHome = (await _clienteProxy.GetImmaginiClienteAsync(idCliente, TipoImmagineDM.Sfondo, false)).FirstOrDefault();
            var imgsGallery = await _clienteProxy.GetImmaginiClienteAsync(idCliente, TipoImmagineDM.Gallery, false);
            var vm = cliente.MapToHomeViewModel(imgHome, imgsGallery);
            vm.Locations = (await _tipologicheProxy.GetLocationsAsync(cliente.Id.Value))?.ToList();
            vm.EventsBaseUrl = string.Format("/{0}/eventi/", cliente.UrlRoute);
            if (cliente.Latitudine.HasValue && cliente.Longitudine.HasValue)
            {
                vm.Latitude = cliente.Latitudine.Value;
                vm.Longitude = cliente.Longitudine.Value;
                vm.GoogleStaticMapUrl = GoogleAPIUtils.GetStaticMapUrl(cliente.Nome, cliente.Latitudine.Value, cliente.Longitudine.Value, _appConfig.GoogleAPI.GoogleMapsAPIKey);
                vm.ExternalGoogleMapUrl = GoogleAPIUtils.GetExternalMapUrl(cliente.Latitudine.Value, cliente.Longitudine.Value);
            }
            else
            {
                //TODO: Gestire il caso in cui non siano presenti le coordinate
            }
            ViewData["AuthToken"] = SecurityUtils.GenerateAuthenticationToken(cliente.UrlRoute, cliente.Id.Value, _appConfig.EncryptKey);
            ViewData["IdCliente"] = cliente.Id;
            vm.DataMinima = DateTime.Now.ToString("yyyy-MM-dd");
            vm.DataMassima = DateTime.Now.AddMonths(2).ToString("yyyy-MM-dd");
            return View(vm);
        }



        [HttpGet]
        [Route("/{cliente}", Name = "HomeCliente")]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _clienteProxy.GetClienteAsync(urlRoute);
            //Se non troviamo il cliente redirect alla home
            if (cliente == null) { return Redirect("/"); }
            //Leggiamo le immagini per il cliente
            var imgHome = (await _clienteProxy.GetImmaginiClienteAsync(cliente.Id.Value, TipoImmagineDM.Sfondo, false)).FirstOrDefault();
            var imgsGallery = await _clienteProxy.GetImmaginiClienteAsync(cliente.Id.Value, TipoImmagineDM.Gallery, false);
            var vm = cliente.MapToHomeViewModel(imgHome, imgsGallery);
            vm.Locations = (await _tipologicheProxy.GetLocationsAsync(cliente.Id.Value))?.ToList();
            vm.EventsBaseUrl = string.Format("/{0}/eventi/", urlRoute);
            if (cliente.Latitudine.HasValue && cliente.Longitudine.HasValue)
            {
                vm.Latitude = cliente.Latitudine.Value;
                vm.Longitude = cliente.Longitudine.Value;
                vm.GoogleStaticMapUrl = GoogleAPIUtils.GetStaticMapUrl(cliente.Nome, cliente.Latitudine.Value, cliente.Longitudine.Value, _appConfig.GoogleAPI.GoogleMapsAPIKey);
                vm.ExternalGoogleMapUrl = GoogleAPIUtils.GetExternalMapUrl(cliente.Latitudine.Value, cliente.Longitudine.Value);
            }
            else
            {
                //TODO: Gestire il caso in cui non siano presenti le coordinate
            }
            ViewData["AuthToken"] = SecurityUtils.GenerateAuthenticationToken(urlRoute, cliente.Id.Value, _appConfig.EncryptKey);
            ViewData["IdCliente"] = cliente.Id;
            vm.DataMinima = DateTime.Now.ToString("yyyy-MM-dd");
            vm.DataMassima = DateTime.Now.AddMonths(2).ToString("yyyy-MM-dd");
            return View(vm);
        }

        [HttpGet("{cliente}/gallery")]
        public async Task<IActionResult> GalleryEdit([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _clienteProxy.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(cliente.Id.Value).IsAtLeastAdmin()) { return Forbid(); }
            ViewData["SASToken"] = SecurityUtils.GenerateSASAuthenticationToken(cliente.Id.Value, cliente.StorageContainer, _appConfig.EncryptKey);
            ViewData["IdCliente"] = cliente.Id.Value;
            var vm = new GalleryEditViewModel();
            vm.ContainerUrl = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                                _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                                cliente.StorageContainer);
            var immagini = await _clienteProxy.GetImmaginiClienteAsync(cliente.Id.Value, TipoImmagineDM.Gallery);
            if (immagini != null)
            {
                foreach (var img in immagini)
                {
                    vm.Images.Add(img);
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
            var imgDeleted = await _clienteProxy.DeleteImmagineGalleryAsync(idCliente, imageId);
            //Cancelliamo i files da Azure
            if (imgDeleted != null)
            {
                if (!string.IsNullOrEmpty(imgDeleted.Url) && (imgDeleted.Url.Contains(_appConfig.Azure.Storage.BlobStorageBaseUrl)))
                {
                    await AzureStorageUtils.DeleteBlobAsync(_appConfig.Azure, imgDeleted.Url);
                }
                if (!string.IsNullOrEmpty(imgDeleted.ThumbnailUrl) && (imgDeleted.ThumbnailUrl.Contains(_appConfig.Azure.Storage.BlobStorageBaseUrl)))
                {
                    await AzureStorageUtils.DeleteBlobAsync(_appConfig.Azure, imgDeleted.ThumbnailUrl);
                }
            }
            return await GalleryEdit(urlRoute);
        }


        //[HttpGet("{cliente}/profilo")]
        //public async Task<IActionResult> ProfileEdit([FromRoute(Name = "cliente")]string urlRoute)
        //{
        //    var cliente = await _apiClient.GetClienteAsync(urlRoute);
        //    //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
        //    var userType = User.GetUserTypeForCliente(cliente.IdCliente);
        //    if (!userType.IsAtLeastAdmin())
        //    {
        //        return Forbid();
        //    }
        //    var vm = cliente.MapToProfileEditVM();
        //    ViewData["IdCliente"] = cliente.IdCliente;
        //    ViewData["SASToken"] = SecurityUtils.GenerateSASAuthenticationToken(cliente.SecurityToken, cliente.StorageContainer, _appConfig.EncryptKey);
        //    ViewData["ContainerUrl"] = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
        //                                        _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
        //                                       cliente.StorageContainer);
        //    ViewData["MapUrl"] = this.BuildMapUrlForCliente(cliente);

        //    return View("Profilo", vm);
        //}

        //[HttpPost("{cliente}/profilo")]
        //public async Task<IActionResult> ProfileEdit([FromRoute(Name = "cliente")]string urlRoute, ClienteProfileEditViewModel profilo)
        //{
        //    float latitudine, longitudine;
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    if (string.IsNullOrEmpty(accessToken))
        //    {
        //        return Forbid();
        //    }
        //    var cliente = await _apiClient.GetClienteAsync(urlRoute);
        //    //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
        //    var userType = User.GetUserTypeForCliente(cliente.IdCliente);
        //    if (!userType.IsAtLeastAdmin())
        //    {
        //        return Forbid();
        //    }
        //    if (!ModelState.IsValid)
        //    {
        //        return View("Profilo", profilo);
        //    }
        //    //NOTA: dato che usando direttamente il tipo float nel ViewModel abbiamo problemi di Culture dobbiamo parsarla a mano
        //    if (!float.TryParse(profilo.Latitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out latitudine) ||
        //        !float.TryParse(profilo.Longitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out longitudine))
        //    {
        //        ModelState.AddModelError(string.Empty, "Coordinate non valide");
        //        return View("Profilo", profilo);
        //    }
        //    //Salviamo il profilo
        //    var apiModel = profilo.MapToAPIModel();
        //    await _apiClient.ClienteSalvaProfilo(cliente.IdCliente, apiModel, accessToken);
        //    return RedirectToAction("ProfileEdit", new { cliente = urlRoute });
        //}



        #region Gestione Utenti del Cliente
        //[HttpGet("{cliente}/users")]
        //public async Task<IActionResult> GetUtentiCliente([FromRoute(Name = "cliente")]string urlRoute)
        //{
        //    var idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
        //    //Verifichiamo che solo gli Admin possano accedere alla pagina di gestione degli utenti
        //    var userType = User.GetUserTypeForCliente(idCliente);
        //    if (!userType.IsAtLeastAdmin())
        //    {
        //        return Forbid();
        //    }
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    var tipologieAbbnamenti = await _apiClient.GetTipologieAbbonamentiClienteAsync(idCliente, accessToken);
        //    var vm = await _apiClient.GetUtentiClienteConAbbonamenti(idCliente, accessToken);
        //    ViewBag.IdCliente = idCliente;
        //    if (tipologieAbbnamenti != null && tipologieAbbnamenti.Count() > 0)
        //    {
        //        ViewBag.TipologieAbbonamenti = tipologieAbbnamenti.Select(ta => new KeyValuePair<int, string>(ta.Id.Value, ta.Nome));
        //    }
        //    return View("Utenti",vm);
        //}
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

        #region PRIVATE STAFF
        private async Task<IEnumerable<SelectListItem>> GetTipologieClientiAsync()
        {
            var allTipologie = await _tipologicheProxy.GetTipologieClientiAsync();
            return allTipologie
                .Select(i => new SelectListItem()
                {
                    Value = i.Id.ToString(),
                    Text = i.Nome
                });
        }
        #endregion
    }
}