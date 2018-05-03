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

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
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
        public async Task<IActionResult> DeleteImage([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name ="imageId")]int imageId)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            var userType = User.GetUserTypeForCliente(cliente.IdCliente);
            if (!userType.IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var urlImage = await _apiClient.DeleteImmagineGalleryAsync(cliente.IdCliente, imageId, accessToken);
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
            //vm.GalleryVM.SASToken = this.GenerateSASAuthenticationToken(cliente.SecurityToken, cliente.StorageContainer);
            //vm.GalleryVM.ContainerUrl = string.Format("{0}{1}{2}", _appConfig.Azure.Storage.BlobStorageBaseUrl,
            //                                    _appConfig.Azure.Storage.BlobStorageBaseUrl.EndsWith("/") ? "" : "/",
            //                                    cliente.StorageContainer);

            return View("Profilo", vm);
        }

        [HttpPost("{cliente}/profilo")]
        public async Task<IActionResult> ProfileEdit([FromRoute(Name = "cliente")]string urlRoute, ClienteProfileEditViewModel cliente)
        {
            return BadRequest(); // Da Implementare
        }

        #region Gestione Locations
        [HttpGet("{cliente}/sale")]
        public async Task<IActionResult> Sale([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            ViewData["IdCliente"] = cliente.IdCliente;
            var locations = await _apiClient.GetLocationsAsync(cliente.IdCliente);
            return View("Sale", locations.ToList());
        }

        [HttpGet("{cliente}/sale/{id}")]
        public async Task<IActionResult> SalaEdit([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name="id")]int idSala)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }

            Models.LocationViewModel location = null;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (idSala > 0)
            {
                location = await _apiClient.GetOneLocationAsync(cliente.IdCliente, idSala, accessToken);
            }
            else
            {
                location = new Models.LocationViewModel();
            }
            return View("Sala", location);
        }


        [HttpPost("{cliente}/sale")]
        public async Task<IActionResult> SalaEdit([FromRoute(Name = "cliente")]string urlRoute, [FromBody] Models.LocationViewModel location)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View("Sala", location);
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            await _apiClient.SaveLocationAsync(cliente.IdCliente, location, accessToken);
            return RedirectToAction("Sale");
        }


        [HttpDelete("{cliente}/sale/{id}")]
        public async Task<IActionResult> SalaDelete([FromRoute(Name = "cliente")]string urlRoute, [FromRoute] int idSala)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            await _apiClient.DeleteOneLocationAsync(cliente.IdCliente, idSala);
            return RedirectToAction("Sale");
        }

        #endregion

        #region Gestione Tipologie Lezioni
        [HttpGet("{cliente}/lezioni")]
        public async Task<IActionResult> Lezioni([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            
            ViewData["IdCliente"] = cliente.IdCliente;
            var lezioni= await _apiClient.GetTipologieLezioniClienteAsync(cliente.IdCliente);
            return View("Lezioni", lezioni.ToList());
        }

        [HttpGet("{cliente}/lezioni/{id:int}")]
        public async Task<IActionResult> LezioneEdit([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "id")]int idLezione)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }

            Models.TipologieLezioniViewModel tipoLezione = null;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (idLezione > 0)
            {
                tipoLezione = await _apiClient.GetOneTipologiaLezione(cliente.IdCliente, idLezione, accessToken);
            }
            if(tipoLezione == null)
            {
                return NotFound();
            }
            return View("Lezione", tipoLezione);
        }

        [HttpGet("{cliente}/lezioni/new")]
        public async Task<IActionResult> LezioneAdd([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }

            Models.TipologieLezioniViewModel tipoLezione = new Models.TipologieLezioniViewModel();
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            return View("Lezione", tipoLezione);
        }


        [HttpPost("{cliente}/lezioni")]
        public async Task<IActionResult> LezioneSave([FromRoute(Name = "cliente")]string urlRoute, [FromForm] Models.TipologieLezioniViewModel tipoLezione)
        {
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(cliente.IdCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View("Lezione", tipoLezione);
            }
            if(tipoLezione.Id.HasValue && (tipoLezione.Id.Value <= 0)) { tipoLezione.Id = null; } 
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            await _apiClient.SaveTipologiaLezioneAsync(cliente.IdCliente, tipoLezione, accessToken);
            return RedirectToAction("Lezioni");
        }

        #endregion
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