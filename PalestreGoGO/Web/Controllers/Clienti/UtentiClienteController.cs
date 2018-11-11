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
        public async Task<IActionResult> GetListaUtenti([FromRoute(Name = "cliente")]string urlRoute)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            ViewData["IdCliente"] = idCliente;
            var utenti = await _apiClient.GetUtentiClienteConAbbonamenti(idCliente, accessToken);
            return View("ListaUtenti", utenti.ToList());

        }

        //#region Gestione Associazione Utenti

        ///// <summary>
        ///// Associa l'utente corrente alla struttura (cliente)
        ///// </summary>
        ///// <param name="urlRoute"></param>
        ///// <returns></returns>
        //[HttpPost("{cliente}/associa")]
        //public async Task<IActionResult> AddAssociazioneUserToCliente([FromRoute(Name = "cliente")]string urlRoute, [FromQuery(Name = "returnUrl")]string returnUrl)
        //{
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    if (string.IsNullOrEmpty(accessToken)) { return Forbid(); }
        //    int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
        //    await _apiClient.ClienteFollowAsync(idCliente, accessToken);
        //    if (!string.IsNullOrWhiteSpace(returnUrl))
        //    {
        //        //Possibile problema di sicurezza? (Open Redirect?)
        //        return Redirect(returnUrl);
        //    }
        //    else
        //    {
        //        return RedirectToAction("Index", new { cliente = urlRoute });
        //    }
        //}

        ///// <summary>
        ///// Associa l'utente corrente alla struttura (cliente)
        ///// </summary>
        ///// <param name="urlRoute"></param>
        ///// <returns></returns>
        //[HttpPost("{cliente}/disassocia")]
        //public async Task<IActionResult> RemoveAssociazioneUserToCliente([FromRoute(Name = "cliente")]string urlRoute)
        //{
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    if (string.IsNullOrEmpty(accessToken)) { return Forbid(); }
        //    int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
        //    await _apiClient.ClienteUnFollowAsync(idCliente, accessToken);
        //    return RedirectToAction("Index", new { cliente = urlRoute });
        //}
        //#endregion

    }
}
