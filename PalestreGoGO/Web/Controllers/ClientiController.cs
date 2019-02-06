using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ready2do.model.common;
using System;
using System.Linq;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models;
using Web.Models.Mappers;
using Web.Services;
using Web.Utils;

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

        /// <summary>
        /// L'action è configurata come AllowAnonymous (anche se in realtà è accessibile soloa gli utenti autenticati) per gestire
        /// esplicitamente il redirect all'action di Signup
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [HttpGet("/register")]
        public async Task<IActionResult> GetRegistrazione()
        {
            //Se l'utente non è Autenticato ==> lo facciamo autenticare usando l'action specifica che valorizza lo State opportunamente
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("SignupCliente", "Account");
            }

            var allTipologie = await _apiClient.GetTipologieClientiAsync();
            ViewBag.TipologieClienti = allTipologie
                .Select(i => new SelectListItem()
                        {
                            Value = i.Id.ToString(),
                            Text = i.Nome
                        });
            var vm = new ClienteRegistrazioneInputModel();
            return View("Registrazione",vm);
        }

        [HttpPost("/register")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> PostRegistrazione(ClienteRegistrazioneInputModel model)
        {
            if (!ModelState.IsValid)
            {
                var allTipologie = await _apiClient.GetTipologieClientiAsync();
                ViewBag.TipologieClienti = allTipologie
                    .Select(i => new SelectListItem()
                    {
                        Value = i.Id.ToString(),
                        Text = i.Nome
                    });
                return View("Registrazione", model);
            }
            //TODO: Salvare il cliente e fare tutti gli step di provisioning

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
                urlIsValid = await _apiClient.CheckUrlRoute(url, idCliente);
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
            var cliente = await _apiClient.GetClienteAsync(idCliente);
            //Se non troviamo il cliente redirect alla home
            if (cliente == null) { return Redirect("/"); }
            //Leggiamo le immagini per il cliente
            var imgHome = (await _apiClient.GetImmaginiClienteAsync(idCliente, TipoImmagineDM.Sfondo, null)).FirstOrDefault();
            var imgsGallery = await _apiClient.GetImmaginiClienteAsync(idCliente, TipoImmagineDM.Gallery, null);
            var vm = cliente.MapToHomeViewModel(imgHome, imgsGallery);
            vm.Locations = (await _apiClient.GetLocationsAsync(cliente.Id.Value))?.ToList();
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
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Se non troviamo il cliente redirect alla home
            if (cliente == null) { return Redirect("/"); }
            //Leggiamo le immagini per il cliente
            var imgHome = (await _apiClient.GetImmaginiClienteAsync(cliente.Id.Value, TipoImmagineDM.Sfondo, null)).FirstOrDefault();
            var imgsGallery = await _apiClient.GetImmaginiClienteAsync(cliente.Id.Value, TipoImmagineDM.Gallery, null);
            var vm = cliente.MapToHomeViewModel(imgHome, imgsGallery);
            vm.Locations = (await _apiClient.GetLocationsAsync(cliente.Id.Value))?.ToList();
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
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(cliente.Id.Value).IsAtLeastAdmin()) { return Forbid(); }
            ViewData["SASToken"] = SecurityUtils.GenerateSASAuthenticationToken(cliente.Id.Value, cliente.StorageContainer, _appConfig.EncryptKey);
            ViewData["IdCliente"] = cliente.Id.Value;
            var vm = new GalleryEditViewModel();
            vm.ContainerUrl = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                                _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                                cliente.StorageContainer);
            var immagini = await _apiClient.GetImmaginiClienteAsync(cliente.Id.Value, TipoImmagineDM.Gallery);
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var imgDeleted = await _apiClient.DeleteImmagineGalleryAsync(idCliente, imageId, accessToken);
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
    }
}