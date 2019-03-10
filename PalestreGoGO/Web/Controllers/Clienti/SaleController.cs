using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Proxies;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [Route("/{cliente}/sale")]
    public class SaleController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly TipologicheProxy _tipologicheProxy;
        private readonly ClienteResolverServices _clientiResolver;

        public SaleController(ILogger<AccountController> logger,
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

        #region Gestione Locations
        [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme, Policy = "CanEditStruttura")]
        [HttpGet()]
        public async Task<IActionResult> ListaSale([FromRoute(Name = "cliente")]string urlRoute)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            ViewData["IdCliente"] = idCliente;
            var locations = await _tipologicheProxy.GetLocationsAsync(idCliente);
            return View("ListaSale", locations.ToList());
        }

        [HttpGet("{id:int}")]
        public async Task<IActionResult> SalaEdit([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "id")]int idSala)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin()) { return Forbid(); }
            ViewData["IdCliente"] = idCliente;
            ViewData["Title"] = "Modifica Sala";
            var location = await _tipologicheProxy.GetOneLocationAsync(idCliente, idSala);
            return View("SalaEdit", location);
        }

        [HttpGet("new")]
        public async Task<IActionResult> SalaNew([FromRoute(Name = "cliente")]string urlRoute)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return RedirectToAction("Index", "Clienti", new { cliente = urlRoute });
            }
            ViewData["IdCliente"] = idCliente;
            ViewData["Title"] = "Nuova Sala";
            return View("SalaEdit", new LocationDM()
            {
                Colore = "#FFFFFF"
            });
        }


        [HttpPost("sale")]
        public async Task<IActionResult> SalaSave([FromRoute(Name = "cliente")]string urlRoute, [FromForm] LocationInputDM location)
        {
            //var cliente = await _apiClient.GetClienteAsync(urlRoute);
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin()) { return Forbid(); }
            if (!ModelState.IsValid)
            {
                if ((location != null) & (location.Id > 0))
                {
                    return View("SalaEdit", location);
                }
            }
            location.IdCliente = idCliente;
            await _tipologicheProxy.SaveLocationAsync(idCliente, location);
            return RedirectToAction("ListaSale", new { cliente = urlRoute });
        }


        [HttpGet("delete/{id:int}")]
        public async Task<IActionResult> SalaDelete([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "id")] int idSala)
        {
            //var cliente = await _apiClient.GetClienteAsync(urlRoute);
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            //Verifichiamo che solo gli Admin possano accedere alla pagina di Edit Sale
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin()) { return Forbid(); }
            await _tipologicheProxy.DeleteOneLocationAsync(idCliente, idSala);
            return RedirectToAction("ListaSale", new { cliente = urlRoute });
        }

        #endregion
    }
}
