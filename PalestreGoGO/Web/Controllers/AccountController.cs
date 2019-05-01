using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Web.Authentication;
using Web.Configuration;
using Web.Filters;
using Web.Models;
using Web.Proxies;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [Route("/accounts")]
    [ServiceFilter(typeof(ReauthenticationRequiredFilter))]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly B2CPolicies _policies;
        private readonly UtentiProxy _utentiProxy;
        private readonly ClienteProxy _clientiProxy;

        public AccountController(ILogger<AccountController> logger, IOptions<AppConfig> apiOptions,
                                UtentiProxy utentiProxy, ClienteProxy clientiProxy, IOptions<B2CPolicies> policies)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _utentiProxy = utentiProxy;
            _clientiProxy = clientiProxy;
            _policies = policies.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null, bool asUser = true)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }
            var authProps = new AuthenticationProperties()
            {
                RedirectUri = returnUrl
            };
            //if (asUser)
            //{
            //    authProps.Items.Add(AzureAdB2COptions.PolicyAuthenticationProperty, _b2cOptions.UserSignUpSignInPolicyId);
            //}
            //else
            //{
            //    authProps.Items.Add(AzureAdB2COptions.PolicyAuthenticationProperty, _b2cOptions.StrutturaSignInPolicyId);
            //}
            //ONLY FOR TEST
            authProps.Items.Add("IsClientRegistration", true.ToString());
            return Challenge(authProps, Constants.OpenIdConnectAuthenticationScheme);
        }

        [AllowAnonymous]
        [HttpGet("signup-cliente")]
        public IActionResult SignupCliente()
        {
            if (!User.Identity.IsAuthenticated)
            {
                var authProps = new AuthenticationProperties()
                {
                    RedirectUri = Url.Action("Registrazione", "Clienti")
                };
                authProps.Items.Add("SignupType", "Cliente");
                authProps.Items.Add(Constants.B2CPolicy, _policies.SignInOrSignUpPolicy);
                return Challenge(authProps, Constants.OpenIdConnectAuthenticationScheme);
            }
            else
            {
                return Redirect("/");
            }
        }
    

        [HttpGet]
        [Route("/logout", Name = "logout")]
        public async Task Logout(string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }
            //NOTA: Aggiungere le properties?
            AuthenticationProperties props = new AuthenticationProperties()
            {
                RedirectUri = returnUrl,
            };
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme, props);
            await HttpContext.SignOutAsync(Constants.OpenIdConnectAuthenticationScheme, props);
        }

        [HttpGet]
        [Route("reset-password")]
        public IActionResult ResetPassword(string returnUrl = null)
        {
            var redirectUrl = returnUrl ?? Url.Action(nameof(HomeController.Index), "Home");
            var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
            properties.Items[Constants.B2CPolicy] = _policies.ResetPasswordPolicy;
            return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        }

        //[HttpGet]
        //public IActionResult UtenteEditProfile(string redirectUrl)
        //{
        //    if (string.IsNullOrWhiteSpace(redirectUrl))
        //    {
        //        redirectUrl = Url.Action(nameof(HomeController.Index), "Home");
        //    }
        //    var properties = new AuthenticationProperties { RedirectUri = redirectUrl };
        //    properties.Items[AzureAdB2COptions.PolicyAuthenticationProperty] = _b2cOptions. AzureAdB2COptions.EditProfilePolicyId;
        //    return Challenge(properties, Constants.OpenIdConnectAuthenticationScheme);
        //}


        /// <summary>
        /// Ritorna la pagina di conferma email
        /// </summary>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("account-to-confirm")]
        [HttpGet()]
        public IActionResult MailToConfirm([FromQuery] string error = null, [FromQuery] bool? isResend = false)
        {
            ViewBag.IsResend = isResend ?? false;
            ViewBag.ErrorMessage = error;
            return View("MailToConfirm");
        }

        [AllowAnonymous]
        [HttpGet("confirm-email")]
        public async Task<IActionResult> ConfermaAccount([FromQuery] string email, [FromQuery] string code)
        {
            try
            {
                var confirmationResult = await _utentiProxy.ConfermaAccount(email, code);
                //TODO: Se si tratta di un cliente, il redirect, deve essere fatto alla home del cliente
                //      In caso di utente invece, il redirect andrebbe fatto alla struttura a cui è affiliato se presente, altrimenti alla home del sito
                if (!confirmationResult.Esito)
                {
                    return RedirectToAction("MailToConfirm", "Impossibile validare l'account.");
                }
                //Aggiungiamo il cookie per indicare che alla prossima request deve essere aggiornato il cookie di autenticazione con le nuove info da B2C
                Response.Cookies.Append(Constants.COOKIE_USERCHANGES_KEY, DateTime.Now.ToString());

                var idCliente = confirmationResult.IdCliente ?? confirmationResult.IdStrutturaAffiliate;
                if (idCliente.HasValue)
                {
                    var cliente = await _clientiProxy.GetClienteAsync(idCliente.Value);
                    var url = Url.RouteUrl("HomeCliente", new { cliente = cliente.UrlRoute });
                    return Redirect(url);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception exc)
            {
                _logger.LogError(exc, $"Errore durante la conferma dell'account. Email:{email}, Code:{code}");
                return BadRequest();
            }
        }

        [HttpPost("send-confirm-email")]
        public async Task<IActionResult> SendNewConfirmMail()
        {
            await _utentiProxy.SendNewConfirmEmail(User.Email());
            return RedirectToAction("MailToConfirm","Account", new { isResend = true });
        }
    }
}