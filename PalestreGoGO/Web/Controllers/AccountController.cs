﻿using Microsoft.AspNetCore.Authentication;
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
using Web.Models;
using Web.Proxies;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [Route("/accounts")]
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

        

        //
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RegisterClienteStep1(ClientRegistrationAccountInputModel model, string returnUrl = null)
        //{
        //    //if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        //    //{
        //    //    returnUrl = null;
        //    //}
        //    //ViewData["ReturnUrl"] = returnUrl;
        //    //if (ModelState.IsValid)
        //    //{

        //    //    /* Creiamo il l'utenza*/
        //    //    var apiModel = new NuovoClienteAPIModel()
        //    //    {
        //    //        Citta = model.Citta,
        //    //        ZipOrPostalCode = model.CAP,
        //    //        Country = model.Country,
        //    //        Email = model.Email,
        //    //        IdTipologia = model.IdTipologia,
        //    //        Indirizzo = model.Indirizzo,
        //    //        Nome = model.Nome,
        //    //        Cognome = model.Cognome,
        //    //        NumTelefono = model.Telefono,
        //    //        RagioneSociale = model.RagioneSociale,
        //    //        UrlRoute = model.URL,
        //    //        NuovoUtente = new NuovoUtenteViewModel()
        //    //        {
        //    //            Cognome = model.Cognome,
        //    //            Email = model.Email,
        //    //            Nome = model.Nome,
        //    //            Password = model.Password,
        //    //            Telefono = model.Telefono
        //    //        }
        //    //    };
        //    //    //Parsing coordinate
        //    //    //NOTA: dato che usando direttamente il tipo float nel ViewModel abbiamo problemi di Culture dobbiamo parsarla a mano
        //    //    if (float.TryParse(model.Latitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitudine) &&
        //    //        float.TryParse(model.Longitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitudine))
        //    //    {
        //    //        apiModel.Coordinate = new CoordinateAPIModel(latitudine, longitudine);
        //    //        var result = await _apiClient.NuovoClienteAsync(apiModel);
        //    //        if (result)
        //    //        {
        //    //            return RedirectToAction("MailToConfirm");
        //    //        }
        //    //        else
        //    //        {
        //    //            //Dobbiamo gestire meglio gli errori lato API!
        //    //            ModelState.AddModelError(string.Empty, "Errore durante la creazione del cliente");
        //    //        }
        //    //    }
        //    //    else
        //    //    {
        //    //        ModelState.AddModelError(string.Empty, "Coordinate non valide");
        //    //    }
        //    //}

        //    // If we got this far, something failed, redisplay form
        //    //return View(await _account.BuildRegisterClienteViewModelAsync(model));
        //    throw new NotImplementedException();
        //}


        //
        // GET: /Account/Register
        //[HttpGet]
        //[AllowAnonymous]
        //public IActionResult RegisterUtente([FromQuery] string returnUrl = null, [FromQuery(Name = "idref")] int? idStrutturaAffiliata = null)
        //{
        //    if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        //    {
        //        returnUrl = null;
        //    }
        //    var vm = new UtenteRegistrationViewModel();
        //    ViewData["ReturnUrl"] = returnUrl;
        //    ViewData["IdAffiliato"] = idStrutturaAffiliata;
        //    return View(vm);
        //}

        //
        //// POST: /Account/Register
        //[HttpPost]
        //[AllowAnonymous]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> RegisterUtente([FromForm]UtenteRegistrationViewModel model, [FromQuery] string returnUrl = null, [FromQuery(Name = "idref")] int? idStrutturaAffiliata = null)
        //{
        //    if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
        //    {
        //        returnUrl = null;
        //    }
        //    ViewData["ReturnUrl"] = returnUrl;
        //    if (ModelState.IsValid)
        //    {
        //        if (model.Password.Equals(model.PasswordConfirm))
        //        {
        //            /* Creiamo il l'utenza*/
        //            var nuovoUtente = new NuovoUtenteViewModel()
        //            {
        //                Cognome = model.Cognome,
        //                Email = model.Email,
        //                Nome = model.Nome,
        //                Password = model.Password
        //            };

        //            var result = await _utentiProxy.NuovoUtenteAsync(nuovoUtente, idStrutturaAffiliata);
        //            if (result)
        //            {
        //                return RedirectToAction("MailToConfirm");
        //            }
        //        }
        //        else
        //        {
        //            ModelState.AddModelError("Password", "Le password inserite non coincidono.");
        //        }
        //    }
        //    else
        //    {
        //        ModelState.AddModelError(string.Empty, "Coordinate non valide");
        //    }
        //    // If we got this far, something failed, redisplay form
        //    return View(model);
        //}

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
        [Route("account-to-confirm", Name = "MailToConfirmRoute")]
        [HttpGet()]
        public IActionResult MailToConfirm([FromQuery] string error)
        {
            return View("MailToConfirm", error);
        }

        [AllowAnonymous]
        [HttpGet("confirm-account")]
        public async Task<IActionResult> ConfermaAccount([FromQuery] string email, [FromQuery] string code)
        {
            try
            {
                var confirmationResult = await _utentiProxy.ConfermaAccount(email, WebUtility.UrlEncode(code));
                //TODO: Se si tratta di un cliente, il redirect, deve essere fatto alla home del cliente
                //      In caso di utente invece, il redirect andrebbe fatto alla struttura a cui è affiliato se presente, altrimenti alla home del sito
                if (!confirmationResult.Esito)
                {
                    return RedirectToAction("MailToConfirm", "Impossibile validare l'account.");
                }
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
    }
}