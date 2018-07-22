using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PalestreGoGo.WebAPIModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Web.Utils;
using Web.Models;
using Web.Services;
using Web.Configuration;
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Web.Models.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class AbbonamentiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;
        private readonly ClienteResolverServices _clientiResolver;

        public AbbonamentiController(ILogger<AccountController> logger,
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

        #region Gestione Tipologie Abbonamenti

        [HttpGet("{cliente}/abbonamenti/tipologie")]
        public async Task<IActionResult> TipoAbbonamenti([FromRoute(Name = "cliente")]string urlRoute)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Profilo
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }

            ViewData["IdCliente"] = idCliente;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var abbonamenti= (await _apiClient.GetTipologieAbbonamentiClienteAsync(idCliente,accessToken)).MapToWebViewModel();
            return View("TipologieAbbonamenti", abbonamenti.ToList());
        }

        [HttpGet("{cliente}/abbonamenti/tipologie/new")]
        public async Task<IActionResult> TipoAbbonamentoAdd([FromRoute(Name = "cliente")]string urlRoute)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            Models.TipologiaAbbonamentoViewModel tipoAbbonamento = new Models.TipologiaAbbonamentoViewModel();
            return View("TipologiaAbbonamentoEdit", tipoAbbonamento);
        }


         
        [HttpGet("{cliente}/abbonamenti/tipologie/{id:int}")]
        public async Task<IActionResult> TipoAbbonamentoEdit([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "id")]int idTipoAbbonamento)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }

            Models.TipologiaAbbonamentoViewModel tipologiaAbbonamento = null;
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (idTipoAbbonamento > 0)
            {
                tipologiaAbbonamento = (await _apiClient.GetOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento, accessToken))
                                        .MapToWebViewModel();
            }
            if (tipologiaAbbonamento == null)
            {
                return NotFound();
            }
            return View("TipologiaAbbonamentoEdit", tipologiaAbbonamento);
        }


        [HttpPost("{cliente}/abbonamenti/tipologie")]
        public async Task<IActionResult> TipoAbbonamentoSave([FromRoute(Name = "cliente")]string urlRoute, [FromForm] Models.TipologiaAbbonamentoViewModel tipoAbbonamento)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return View("TipologiaAbbonamentoEdit", tipoAbbonamento);
            }
            //if (tipoAbbonamento.Id.HasValue && (tipoAbbonamento.Id.Value <= 0)) { tipoAbbonamento.Id = null; }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            await _apiClient.SaveTipologiaAbbonamentoAsync(idCliente, tipoAbbonamento.MapToAPIModel(), accessToken);
            return RedirectToAction("TipoAbboanmenti");
        }

        [HttpGet("{cliente}/abbonamenti/tipologie/{id}/delete")]
        public async Task<IActionResult> TipoAbbonamentoDelete([FromRoute(Name = "cliente")]string urlRoute, [FromRoute] int idTipoAbbonamento)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            await _apiClient.DeleteOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento, accessToken);
            return RedirectToAction("TipoAbboanmenti");
        }

        #endregion


        #region Gestione Abbonamenti Utenti
        //TODO: da implementare
        [HttpGet("{cliente}/abbonamenti/add/{userId:guid}")]
        public async Task<IActionResult> AddAbbonamentoToUser([FromRoute(Name = "cliente")]string urlRoute, 
                                                              [FromRoute(Name = "userId")]Guid userId, 
                                                              [FromQuery(Name ="tipoAbb")] int idTipoAbbonamento)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Gestione Abbonamenti
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            var tipoAbbonamento = await _apiClient.GetOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento, accessToken);
            if(tipoAbbonamento == null) { return BadRequest(); }

            EditAbbonamentoUtenteInputModel vm = new EditAbbonamentoUtenteInputModel()
            {
                Id = null,
                IdCliente = idCliente,
                IdUtente = userId,
                DataInizioValidita = DateTime.Now,
                IdTipologiaAbbonamento = idTipoAbbonamento,
                IngressiResidui = tipoAbbonamento.NumIngressi,
                Scadenza = tipoAbbonamento.DurataMesi.HasValue ? DateTime.Now.AddMonths(tipoAbbonamento.DurataMesi.Value): (DateTime?)null,
                ScadenzaCertificato = null,
                Pagato = false
            };

            ViewBag.IdCliente = idCliente;
            ViewBag.IdUtente = userId;
            ViewBag.ClienteUrl = urlRoute;
            ViewBag.TipologiaAbbonamento = tipoAbbonamento.Nome;

            return View("NuovoAbbonamento", vm);
        }

        [HttpPost("{cliente}/abbonamenti/add/{userId:guid}")]
        public async Task<IActionResult> AddAbbonamentoToUser([FromRoute(Name = "cliente")]string urlRoute,
                                                              [FromRoute(Name = "userId")]Guid userId,
                                                              [FromForm]EditAbbonamentoUtenteInputModel model)
        {
            if (!ModelState.IsValid)
            {
                return View("NuovoAbbonamento", model);
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if((model.IdCliente != idCliente) || (model.IdUtente != userId)) { return BadRequest(); }
            await _apiClient.EditAbbonamentoCliente(idCliente, model.MapToAPIModel(), accessToken);
            return RedirectToAction("GetUtentiCliente", "Clienti",new { cliente = urlRoute});
        }

        #endregion
    }
}