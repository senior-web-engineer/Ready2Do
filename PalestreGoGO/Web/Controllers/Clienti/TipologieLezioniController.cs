using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models.Mappers;
using Web.Services;
using Web.Utils;

namespace Web.Controllers.Clienti
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [Route("/{cliente}/lezioni")]
    public class TipologieLezioniController: Controller
    {
        private readonly ILogger<TipologieLezioniController> _logger;
        private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;
        private readonly ClienteResolverServices _clientiResolver;

        public TipologieLezioniController(ILogger<TipologieLezioniController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 WebAPIClient apiClient,
                                 ClienteResolverServices clientiResolver)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
            _clientiResolver = clientiResolver;
        }


        #region Gestione Tipologie Lezioni
        [HttpGet]
        public async Task<IActionResult> ListaLezioni([FromRoute(Name = "cliente")]string urlRoute)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            ViewData["IdCliente"] = idCliente;
            var lezioni = await _apiClient.GetTipologieLezioniClienteAsync(idCliente);
            return View("ListaLezioni", lezioni.ToVM());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> LezioneEdit([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "id")]int idLezione)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            ViewData["IdCliente"] = idCliente;

            Models.TipologieLezioniViewModel tipoLezione = null;
            if (idLezione > 0)
            {
                tipoLezione = (await _apiClient.GetOneTipologiaLezione(idCliente, idLezione)).ToVM();
            }
            if (tipoLezione == null)
            {
                return NotFound();
            }
            return View("LezioneEdit", tipoLezione);
        }

        [HttpGet("new")]
        public async Task<IActionResult> LezioneAdd([FromRoute(Name = "cliente")]string urlRoute)
        {
            //var cliente = await _apiClient.GetClienteAsync(urlRoute);
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            ViewData["IdCliente"] = idCliente;
            Models.TipologieLezioniViewModel tipoLezione = new Models.TipologieLezioniViewModel();
            return View("LezioneEdit", tipoLezione);
        }


        [HttpPost]
        public async Task<IActionResult> LezioneSave([FromRoute(Name = "cliente")]string urlRoute, [FromForm] Models.TipologieLezioniViewModel tipoLezione)
        {
            //var cliente = await _apiClient.GetClienteAsync(urlRoute);
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            ViewData["IdCliente"] = idCliente;
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View("LezioneEdit", tipoLezione);
            }
            if (tipoLezione.Id.HasValue && (tipoLezione.Id.Value <= 0)) { tipoLezione.Id = null; }
            await _apiClient.SaveTipologiaLezioneAsync(idCliente, tipoLezione.ToDM());
            return RedirectToAction("ListaLezioni");
        }

        [HttpGet("delete/{id}")]
        public async Task<IActionResult> LezioneDelete([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name ="id")] int idLezione)
        {
            //var cliente = await _apiClient.GetClienteAsync(urlRoute);
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            await _apiClient.DeleteOneTipologiaLezioneAsync(idCliente, idLezione);
            return RedirectToAction("ListaLezioni");
        }

        [Produces("application/json")]
        [HttpGet("checkname")]
        public async Task<IActionResult> CheckNome([FromRoute(Name = "cliente")]string urlRoute, [FromQuery(Name = "Nome")]string nome, int? IdCliente, int? id)
        {
            _logger.LogDebug($"Begin CheckNome({urlRoute},{nome},{IdCliente},{id}) - Path: {ControllerContext.HttpContext.Request.Path}?{ControllerContext.HttpContext.Request.QueryString}");
            //int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            bool isValid = await _apiClient.CheckNameTipologiaLezioneAsync(IdCliente.Value, nome, accessToken, id);
            if (isValid)
            {
                return Json(data: true);
            }
            else
            {
                return Json(data: "Esiste già una Tipologia di lezione con lo stesso nome");
            }
        }

        #endregion
    }
}
