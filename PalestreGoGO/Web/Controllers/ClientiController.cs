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

namespace Web.Controllers
{
    [Authorize]
    public class ClientiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private WebAPIClient _apiClient;

        public ClientiController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 WebAPIClient apiClient
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            var locations = await _apiClient.GetLocationsAsync(cliente.IdCliente);

            ViewData["ReturnUrl"] = Request.Path.ToString();
            ViewData["Sale"] = locations;
            ViewData["AuthToken"] = GenerateAuthenticationToken(urlRoute, cliente.IdCliente);
            ViewData["ClienteRoute"] = urlRoute;
            //ViewData["UserRole"] = User.GetUserRoleForCliente(cliente.IdCliente);
            return View(cliente.MapToHomeViewModel());
        }


        [HttpGet("{cliente}/gallery")]
        public async Task<IActionResult> GalleryEdit([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin()) { return Forbid(); }
            ViewData["SASToken"] = GenerateSASAuthenticationToken(cliente.SecurityToken, cliente.StorageContainer);
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
        public async Task<IActionResult> DeleteImage([FromRoute(Name = "cliente")]string urlRoute)
        {
            return await ProfileEdit(urlRoute);
        }


        [HttpGet("{cliente}/profile")]
        public async Task<IActionResult> ProfileEdit([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            var userType = User.GetUserTypeForCliente(cliente.IdCliente);
            if ((userType != UserType.Admin) && (userType != UserType.GlobalAdmin)) { return Forbid(); }
            var vm = new ClienteProfileEditViewModel();
            vm.GalleryVM.SASToken = this.GenerateSASAuthenticationToken(cliente.SecurityToken, cliente.StorageContainer);
            vm.GalleryVM.ContainerUrl = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
                                                _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
                                                cliente.StorageContainer);
            if (cliente.Immagini != null)
            {
                foreach (var img in cliente.Immagini)
                {
                    vm.GalleryVM.Images.Add(new ImageViewModel()
                    {
                        Id = img.Id,
                        Alt = img.Alt,
                        Caption = img.Nome,
                        Url = img.Url,
                        Ordinamento = img.Ordinamento
                    });
                }
            }
            //TODO: Completare popolamento VM
            return View("Profile", vm);
        }


        #region Helpers
        /// <summary>
        /// Genera una stringa rappresentante un "token" per l'autenticazione delle chiamate Ajax
        /// </summary>
        /// <param name="secuirtyToken"></param>
        /// <param name="storageContainer"></param>
        /// <returns></returns>
        private string GenerateSASAuthenticationToken(string secuirtyToken, string storageContainer)
        {
            var token = new SASTokenModel()
            {
                SecurityToken = secuirtyToken,
                ContainerName = storageContainer,
                CreationTime = DateTime.Now
            };
            string json = JsonConvert.SerializeObject(token, Formatting.None);
            //Cifriamo il json ottenuto
            var result = SecurityUtils.EncryptStringWithAes(json, Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
            return result;
        }

        private string GenerateAuthenticationToken(string clientRouteUrl, int idCliente)
        {
            var token = new AuthTokenModel()
            {
                ClientRoute = clientRouteUrl,
                CreationTime = DateTime.Now,
                IdCliente = idCliente
            };
            string json = JsonConvert.SerializeObject(token, Formatting.None);
            //Cifriamo il json ottenuto
            var result = SecurityUtils.EncryptStringWithAes(json, Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
            return result;
        }
        #endregion
    }
}