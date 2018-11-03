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
using Web.Services;
using Web.Utils;

namespace Web.Controllers.Clienti
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
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
            return View("ListaLezioni", lezioni.ToList());
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (idLezione > 0)
            {
                tipoLezione = await _apiClient.GetOneTipologiaLezione(idCliente, idLezione, accessToken);
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            await _apiClient.SaveTipologiaLezioneAsync(idCliente, tipoLezione, accessToken);
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
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            await _apiClient.DeleteOneTipologiaLezioneAsync(idCliente, idLezione, accessToken);
            return RedirectToAction("ListaLezioni");
        }

        #endregion
    }
}
