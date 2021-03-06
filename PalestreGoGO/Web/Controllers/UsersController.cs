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
using Web.Filters;
using Web.Models;
using Web.Models.Mappers;
using Web.Proxies;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [Route("me")]
    [ServiceFilter(typeof(ReauthenticationRequiredFilter))]

    public class UsersController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly UtentiProxy _utentiProxy;
        private readonly ClienteResolverServices _clientiResolver;

        public UsersController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 UtentiProxy utentiProxy,
                                 ClienteResolverServices clientiResolver)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _utentiProxy = utentiProxy;
            _clientiResolver = clientiResolver;
        }

        //Ritorna il profilo dell'utente
        [HttpGet("profilo")]
        public async Task<IActionResult> GetProfilo()
        {
            var userId = User.UserId();
            if (string.IsNullOrWhiteSpace(userId) ) { return Forbid(); }

            var utenteDM  = await _utentiProxy.GetProfiloUtente();
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
            await _utentiProxy.SalvaProfiloUtente(profilo);
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
            await _utentiProxy.ClienteFollowAsync(idCliente);
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
            await _utentiProxy.ClienteUnFollowAsync(idCliente);
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