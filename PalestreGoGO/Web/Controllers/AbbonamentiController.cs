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
using Web.Models.Mappers;
using System.Threading;
using Web.Proxies;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    public class AbbonamentiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly TipologicheProxy _tipologicheProxy;
        private readonly ClienteResolverServices _clientiResolver;

        public AbbonamentiController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 TipologicheProxy tipologicheProxy,
                                 ClienteResolverServices clientiResolver
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _tipologicheProxy = tipologicheProxy;
            _clientiResolver = clientiResolver;
        }

        #region Gestione Tipologie Abbonamenti

        [HttpGet("{cliente}/abbonamenti/tipologie/checkname")]
        [Produces("application/json")]
        public async Task<IActionResult> CheckNome([FromRoute(Name = "cliente")]string urlRoute, [FromQuery(Name = "Nome")]string nomeAbbonamento, int? id)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            //TODO:Modificare implementazione aggiungendo una API apposita invece di farsi ritornare tutti gli abbonamenti per il Cliente
            bool nomeIsValid = await _tipologicheProxy.CheckNomeTipologiaAbbonamentoAsync(idCliente, nomeAbbonamento, id);
            if (!nomeIsValid)
            {
                return Json(data: $"Esiste già una tipologia di abbonamento con lo stesso nome.");
            }
            else
            {
                return Json(data: nomeIsValid);
            }
        }

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
            var abbonamenti = (await _tipologicheProxy.GetTipologieAbbonamentiClienteAsync(idCliente)).MapToWebViewModel();
            return View("ListaTipologieAbbonamenti", abbonamenti.ToList());
        }

        [HttpGet("{cliente}/abbonamenti/tipologie/new")]
        public async Task<IActionResult> TipoAbbonamentoNew([FromRoute(Name = "cliente")]string urlRoute)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            ViewData["IdCliente"] = idCliente;
            ViewData["Title"] = "Nuovo tipo bbonamento";
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            Models.TipologiaAbbonamentoViewModel tipoAbbonamento = new Models.TipologiaAbbonamentoViewModel();
            return View("TipologiaAbbonamentoEdit", tipoAbbonamento);
        }

        [Produces("application/json")]
        [HttpGet("api/{idCliente:int}/abbonamenti/tipologie/{id:int}")]
        public async Task<IActionResult> GetTipologiaAbbonamentoJson([FromRoute(Name = "idCliente")]int idCliente, [FromRoute(Name = "id")]int idTipoAbbonamento)
        {
            TipologiaAbbonamentoViewModel tipologiaAbbonamento = null;
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            if (idTipoAbbonamento > 0)
            {
                tipologiaAbbonamento = (await _tipologicheProxy.GetOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento))
                                        .MapToWebViewModel();
            }
            if (tipologiaAbbonamento == null)
            {
                return NotFound();
            }
            return Json(tipologiaAbbonamento);
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
            ViewData["IdCliente"] = idCliente;
            ViewData["Title"] = "Modifica tipo abbonamento";
            Models.TipologiaAbbonamentoViewModel tipologiaAbbonamento = null;
            if (idTipoAbbonamento > 0)
            {
                tipologiaAbbonamento = (await _tipologicheProxy.GetOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento))
                                        .MapToWebViewModel();
            }
            if (tipologiaAbbonamento == null)
            {
                return NotFound();
            }
            Thread.CurrentThread.CurrentUICulture = System.Globalization.CultureInfo.InvariantCulture;
            return View("TipologiaAbbonamentoEdit", tipologiaAbbonamento);
        }


        [HttpPost("{cliente}/abbonamenti/tipologie")]
        public async Task<IActionResult> TipoAbbonamentoSave([FromRoute(Name = "cliente")]string urlRoute, [FromForm] TipologiaAbbonamentoViewModel tipoAbbonamento)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            ViewData["IdCliente"] = idCliente;
            if (!ModelState.IsValid)
            {
                return View("TipologiaAbbonamentoEdit", tipoAbbonamento);
            }
            //if (tipoAbbonamento.Id.HasValue && (tipoAbbonamento.Id.Value <= 0)) { tipoAbbonamento.Id = null; }
            await _tipologicheProxy.SaveTipologiaAbbonamentoAsync(idCliente, tipoAbbonamento.MapToAPIModel());
            return RedirectToAction("TipoAbbonamenti");
        }

        [HttpGet("{cliente}/abbonamenti/tipologie/delete/{id}")]
        public async Task<IActionResult> TipoAbbonamentoDelete([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "id")] int idTipoAbbonamento)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            ViewData["IdCliente"] = idCliente;
            await _tipologicheProxy.DeleteOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento);
            return RedirectToAction("TipoAbbonamenti");
        }

        #endregion

    }
}