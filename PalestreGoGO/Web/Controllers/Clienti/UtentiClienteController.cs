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
using Web.Models;
using Web.Services;
using Web.Utils;
using Web.Views.UtentiCliente;

namespace Web.Controllers.Clienti
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    [Route("/{cliente}/users")]
    public class UtentiClienteController : Controller
    {
        private readonly ILogger<UtentiClienteController> _logger;
        //private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;
        private readonly ClienteResolverServices _clientiResolver;

        public UtentiClienteController(ILogger<UtentiClienteController> logger,
                                         //IOptions<AppConfig> apiOptions,
                                         WebAPIClient apiClient,
                                         ClienteResolverServices clientiResolver)
        {
            _logger = logger;
            _apiClient = apiClient;
            _clientiResolver = clientiResolver;
        }

        [HttpGet()]
        public async Task<IActionResult> GetListaUtenti([FromRoute(Name = "cliente")]string urlRoute, [FromQuery(Name = "incstato")]bool includeStatus = true,
                                                        [FromQuery(Name = "sortby")]string sortby = "Cognome", [FromQuery(Name = "asc")]bool asc = true)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            ViewData["IdCliente"] = idCliente;
            var utenti = await _apiClient.GetUtentiCliente(idCliente, accessToken, includeStatus, page: 1, pageSize: 2000, sortBy: sortby, asc: asc);
            return View("ListaUtenti", utenti.ToList());
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUtente([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "userId")]string userId, [FromQuery(Name ="tabId")]int tabId = 0)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            ViewData["IdCliente"] = idCliente;
            ViewData["TabId"] = tabId;
            var utente = await _apiClient.GetUtenteCliente(idCliente, userId, accessToken);

            return View("DettaglioUtenteCliente", utente);
        }

        #region Gestione Abbonamenti Utenti
        ////TODO: da implementare
        //[HttpGet("{userId}/abbonamenti/add")]
        //public async Task<IActionResult> AddAbbonamentoToUser([FromRoute(Name = "cliente")]string urlRoute,
        //                                                      [FromRoute(Name = "userId")]string userId,
        //                                                      [FromQuery(Name = "tipoAbb")] int idTipoAbbonamento)
        //{
        //    int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
        //    //Verifichiamo che solo gli Admin possano accedere alla pagina di Gestione Abbonamenti
        //    if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
        //    {
        //        return Forbid();
        //    }
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    var tipoAbbonamento = await _apiClient.GetOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento, accessToken);
        //    if (tipoAbbonamento == null) { return BadRequest(); }

        //    EditAbbonamentoUtenteInputModel vm = new EditAbbonamentoUtenteInputModel()
        //    {
        //        Id = null,
        //        IdCliente = idCliente,
        //        IdUtente = userId,
        //        DataInizioValidita = DateTime.Now,
        //        IdTipoAbbonamento = idTipoAbbonamento,
        //        IngressiIniziali = tipoAbbonamento.NumIngressi,
        //        IngressiResidui = tipoAbbonamento.NumIngressi,
        //        Scadenza = tipoAbbonamento.DurataMesi.HasValue ? DateTime.Now.AddMonths(tipoAbbonamento.DurataMesi.Value) : (DateTime?)null,
        //        Importo = tipoAbbonamento?.Costo ?? 0,
        //    };

        //    ViewBag.IdCliente = idCliente;
        //    ViewBag.IdUtente = userId;
        //    ViewBag.ClienteUrl = urlRoute;
        //    ViewBag.TipologiaAbbonamento = tipoAbbonamento.Nome;

        //    return View("NuovoAbbonamento", vm);
        //}

        [HttpPost("{userId}/abbonamenti/add")]
        public async Task<IActionResult> AddAbbonamentoToUser([FromRoute(Name = "cliente")]string urlRoute,
                                                              [FromRoute(Name = "userId")]string userId,
                                                              [FromForm]AbbonamentoUtenteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ViewComponent(typeof(UtenteAddAbbonamentoViewComponent), model);
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if ((model.IdCliente != idCliente) || (model.IdUtente != userId)) { return BadRequest(); }
            await _apiClient.EditAbbonamentoCliente(idCliente, model.MapToAPIModel(), accessToken);
            return RedirectToAction("GetUtentiCliente", "Clienti", new { cliente = urlRoute });
        }

        #endregion

    }
}
