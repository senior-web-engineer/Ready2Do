using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using ready2do.model.common;
using Web.Configuration;
using Web.Models;
using Web.Models.Mappers;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    [Route("me")]
    public class UsersController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;
        private readonly ClienteResolverServices _clientiResolver;

        public UsersController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 WebAPIClient apiClient,
                                 ClienteResolverServices clientiResolver)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
            _clientiResolver = clientiResolver;
        }

        //Ritorna il profilo dell'utente
        [HttpGet("profilo")]
        public async Task<IActionResult> GetProfilo()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.UserId();
            if (string.IsNullOrWhiteSpace(userId) || string.IsNullOrWhiteSpace(accessToken)) { return Forbid(); }

            var utenteDM  = await _apiClient.GetProfiloUtente(accessToken);
            var vm = UserProfileViewModel.FromUtenteDM(utenteDM);
            vm.Email = User.Email();
            vm.EMailConfirmed = User.EmailConfirmedOn().HasValue;
            return View("Profilo", vm);
        }



        [HttpPost("profilo")]
        public async Task<IActionResult> SalvaProfilo(UtenteInputDM profilo)
        {
            if (profilo == null) return BadRequest();
            if (!ModelState.IsValid)
            {
                var model = UserProfileViewModel.FromUtenteDM(profilo);
                model.Email = User.Email();
                model.EMailConfirmed = User.EmailConfirmedOn().HasValue;
                model.TelephoneConfirmed = User.TelephoneConfirmedOn().HasValue;
                return View("Profilo", model);
            }
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            await _apiClient.SalvaProfiloUtente(profilo, accessToken);
            return RedirectToAction("GetProfilo");
        }


        #region Gestione Associazione Utenti

        /// <summary>
        /// Associa l'utente corrente alla struttura (cliente)
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <returns></returns>
        [HttpPost("associa/{idCliente:int}")]
        public async Task<IActionResult> AssociaToCliente([FromRoute]int idCliente, [FromQuery(Name = "returnUrl")]string returnUrl)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken)) { return Forbid(); }
            //            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _apiClient.ClienteFollowAsync(idCliente, accessToken);
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                //Possibile problema di sicurezza? (Open Redirect?)
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", new { idCliente = idCliente });
            }
        }

        /// <summary>
        /// Associa l'utente corrente alla struttura (cliente)
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <returns></returns>
        [HttpPost("disassocia/{idCliente:int}")]
        public async Task<IActionResult> RemoveAssociazioneToCliente([FromRoute]int idCliente, [FromQuery(Name = "returnUrl")]string returnUrl)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken)) { return Forbid(); }
            //          int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _apiClient.ClienteUnFollowAsync(idCliente, accessToken);
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                //Possibile problema di sicurezza? (Open Redirect?)
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", new { idCliente = idCliente });
            }
        }
        #endregion

    }
}