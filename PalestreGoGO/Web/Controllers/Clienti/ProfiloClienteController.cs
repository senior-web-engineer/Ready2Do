using System;
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
using Web.Models.Mappers;
using ready2do.model.common;
using Web.Proxies;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [Route("/{cliente}/profilo")]
    public class ProfiloClienteController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly ClienteProxy _apiClient;
        private readonly ClienteResolverServices _clientiResolver;

        public ProfiloClienteController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 ClienteProxy apiClient,
                                 ClienteResolverServices clientiResolver
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
            _clientiResolver = clientiResolver;
        }

        #region GESTIONE BANNER
        [HttpGet]
        [Route("banner")]
        public async Task<IActionResult> GetBanner([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Se non troviamo il cliente redirect alla home
            if (cliente == null) { return RedirectToAction("Index", "Home", new { cliente = urlRoute }); }
            if (!User.GetUserTypeForCliente(cliente.Id.Value).IsAtLeastAdmin())
            {
                return RedirectToAction("Index", "Clienti", new { cliente = urlRoute });
            }
            ViewData["IdCliente"] = cliente.Id;
            //ViewData["SASToken"] = SecurityUtils.GenerateSASAuthenticationToken(cliente.SecurityToken, cliente.StorageContainer, _appConfig.EncryptKey);
            ViewData["ContainerUrl"] = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                    _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                   cliente.StorageContainer);
            var immagine = (await _apiClient.GetImmaginiClienteAsync(idCliente: cliente.Id.Value, tipoImmagini: TipoImmagineDM.Sfondo)).Single();
            ClienteHeaderViewModel vm = new ClienteHeaderViewModel()
            {
                IdCliente = cliente.Id.Value,
                ImmagineHome = immagine
            };
            return View("BannerEdit", vm);
        }

        [HttpPost]
        [Route("banner")]
        public async Task<IActionResult> SaveBanner([FromRoute(Name = "cliente")]string urlRoute, [FromForm] ClienteHeaderInputViewModel model)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            ViewData["IdCliente"] = idCliente;
            var image = new ImmagineClienteInputDM()
            {
                Id = model.IdImmagine,
                IdTipoImmagine = (int)TipoImmagineDM.Sfondo,
                Url = model.UrlImmagineHome,
                Nome = "Image Header",
                IdCliente = idCliente
            };
            await _apiClient.ClienteSalvaBanner(idCliente, image);
            return RedirectToAction("GetBanner", new { cliente = urlRoute });
        }
        #endregion

        #region GESTIONE ANAGRAFICA
        [HttpGet]
        [Route("anagrafica")]
        public async Task<IActionResult> GetAnagrafica([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Se non troviamo il cliente redirect alla home
            if (cliente == null) { return RedirectToAction("Index", "Home"); }
            if (!User.GetUserTypeForCliente(cliente.Id.Value).IsAtLeastAdmin())
            {
                return RedirectToAction("Index", "Clienti", new { cliente = urlRoute });
            }
            ViewData["IdCliente"] = cliente.Id.Value;
            ViewData["GoogleMapsAPIUrl"] = GoogleAPIUtils.GetGoogleMapsAPIUrl(_appConfig.GoogleAPI.GoogleMapsAPIKey);
            //ViewData["UrlGoolePlaces"] = GoogleAPIUtils.GetGooglePlacesAPIUrl(_appConfig.GoogleAPI.GoogleMapsAPIKey);

            return View("Anagrafica", cliente.ToAnagraficaClienteViewModel());
        }

        [HttpPost]
        [Route("anagrafica")]
        public async Task<IActionResult> SaveAnagrafica([FromRoute(Name = "cliente")]string urlRoute, AnagraficaClienteEditViewModel model)
        {
            float latitudine, longitudine;
            var idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }

            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            var userType = User.GetUserTypeForCliente(idCliente);
            if (!userType.IsAtLeastAdmin()) { return Forbid(); }
            if (!ModelState.IsValid) { return View("SaveAnagrafica", model); }


            //NOTA: dato che usando direttamente il tipo float nel ViewModel abbiamo problemi di Culture dobbiamo parsarla a mano
            if (!float.TryParse(model.Latitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out latitudine) ||
                !float.TryParse(model.Longitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out longitudine))
            {
                ModelState.AddModelError(string.Empty, "Coordinate non valide");
                return View("Profilo", model);
            }

            var apiModel = model.ToApiModel();
            await _apiClient.ClienteSalvaAnagrafica(idCliente, apiModel);
            //Redirect utilizzando il nuovo urlRoute
            return RedirectToAction("GetAnagrafica", new { cliente = model.UrlRoute });
        }
        #endregion

        #region GESTIONE ORARIO APERTURA
        [HttpGet]
        [Route("orario")]
        public async Task<IActionResult> GetOrarioApertura([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Se non troviamo il cliente redirect alla home
            if (cliente == null) { return RedirectToAction("Index", "Home", new { cliente = urlRoute }); }
            if (!User.GetUserTypeForCliente(cliente.Id.Value).IsAtLeastAdmin())
            {
                return RedirectToAction("Index", "Clienti", new { cliente = urlRoute });
            }
            ViewData["IdCliente"] = cliente.Id;
            ViewData["GoogleMapsAPIUrl"] = GoogleAPIUtils.GetGoogleMapsAPIUrl(_appConfig.GoogleAPI.GoogleMapsAPIKey);
            var vm = cliente.OrarioApertura != null ? cliente.OrarioApertura.MapOrarioApertura() : new Models.OrarioAperturaViewModel();
            return View("OrariApertura", vm);
        }

        [HttpPost]
        [Route("orario")]
        public async Task<IActionResult> SaveOrarioApertura([FromRoute(Name = "cliente")]string urlRoute, Models.OrarioAperturaViewModel model)
        {
            var idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            var userType = User.GetUserTypeForCliente(idCliente);
            if (!userType.IsAtLeastAdmin()) { return Forbid(); }
            if (!ModelState.IsValid) { return View("GetOrarioApertura", model); }

            var apiModel = model.MapToApiModel();
            await _apiClient.ClienteSalvaOrarioApertura(idCliente, apiModel);
            return RedirectToAction("GetOrarioApertura", new { cliente = urlRoute });
        }
        #endregion

        #region GESTIONE GALLERY
        [HttpGet("gallery")]
        public async Task<IActionResult> GalleryEdit([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(cliente.Id.Value).IsAtLeastAdmin()) { return Forbid(); }
            //ViewData["SASToken"] = SecurityUtils.GenerateSASAuthenticationToken(cliente.Id.Value, cliente.StorageContainer, _appConfig.EncryptKey);
            ViewData["IdCliente"] = cliente.Id.Value;
            var vm = new GalleryEditViewModel();
            vm.ContainerUrl = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                                _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                                cliente.StorageContainer);
            var immagini = await _apiClient.GetImmaginiClienteAsync(cliente.Id.Value, TipoImmagineDM.Gallery);
            if (immagini != null)
            {
                foreach (var img in immagini.OrderBy(img=>img.Ordinamento))
                {
                    vm.Images.Add(img);
                }
            }
            return View("Gallery", vm);
        }

        [HttpGet("gallery/{id:int}")]
        public async Task<IActionResult> EditImageGallery([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name ="id")]int idImage)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            ViewData["IdCliente"] = cliente.Id;
            ViewData["ContainerUrl"] = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                   _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                  cliente.StorageContainer);
            ImmagineClienteDM vm;
            if(idImage > 0)
            {
                vm = await _apiClient.GetImmagineClienteAsync(cliente.Id.Value, idImage);
            }
            else
            {
                vm = new ImmagineClienteDM()
                {
                    IdTipoImmagine = (int)TipoImmagineDM.Gallery,
                    Ordinamento = -1 //Lo impostiamo a -1 e sarà valorizzato in fase di salvataggio
                };
            }
            return View("EditImageGallery", vm);
        }

        [HttpPost("gallery")]
        public async Task<IActionResult> SaveImageGallery([FromRoute(Name = "cliente")]string urlRoute, ImmagineClienteDM immagine)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!ModelState.IsValid)
            {
                var cliente = await _apiClient.GetClienteAsync(urlRoute);
                ViewData["IdCliente"] = cliente.Id;
                ViewData["ContainerUrl"] = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                       _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                      cliente.StorageContainer);
                return View("EditImageGallery", immagine);
            }
            immagine.IdCliente = idCliente;
            await _apiClient.GallerySalvaImmagine(idCliente, immagine);
            return RedirectToAction("GalleryEdit", new { cliente=urlRoute});
        }

        [HttpDelete("gallery/delete/{imageId}")]
        public async Task<IActionResult> DeleteImage([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "imageId")]int imageId)
        {
            //var cliente = await _apiClient.GetClienteAsync(urlRoute);
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            var userType = User.GetUserTypeForCliente(idCliente);
            if (!userType.IsAtLeastAdmin())
            {
                return Forbid();
            }
            var imgDeleted = await _apiClient.DeleteImmagineGalleryAsync(idCliente, imageId);
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

        [HttpPost("gallery/order")]
        public async Task<IActionResult> ChangeImageGalleryOrder([FromRoute(Name = "cliente")]string urlRoute, [FromBody]int[] newOrder)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _apiClient.GalleryChangeOrder(idCliente, newOrder);
            return Ok();
        }
        #endregion
    }
}