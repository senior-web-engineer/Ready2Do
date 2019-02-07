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
using Web.Configuration;
using Web.Models;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly AzureAdB2COptions _b2cOptions;

        private readonly WebAPIClient _apiClient;

        public AccountController(ILogger<AccountController> logger, IOptions<AppConfig> apiOptions, WebAPIClient apiClient, IOptions<AzureAdB2COptions> b2cOptions)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
            _b2cOptions = b2cOptions.Value;
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
            if (asUser)
            {
                authProps.Items.Add(AzureAdB2COptions.PolicyAuthenticationProperty, _b2cOptions.UserSignUpSignInPolicyId);
            }
            else
            {
                authProps.Items.Add(AzureAdB2COptions.PolicyAuthenticationProperty, _b2cOptions.StrutturaSignInPolicyId);
            }
            //ONLY FOR TEST
            authProps.Items.Add("IsClientRegistration", true.ToString());
            return Challenge(authProps, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [AllowAnonymous]
        public IActionResult SignupCliente()
        {
            var authProps = new AuthenticationProperties()
            {
                RedirectUri = Url.Action("GetRegistrazione", "Clienti")
            };
            authProps.Items.Add("SignupType", "Cliente");
            authProps.Items.Add(AzureAdB2COptions.PolicyAuthenticationProperty, "B2C_1_SigninSignup");
            return Challenge(authProps, OpenIdConnectDefaults.AuthenticationScheme);
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
            await HttpContext.SignOutAsync(OpenIdConnectDefaults.AuthenticationScheme, props);
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
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterUtente([FromQuery] string returnUrl = null, [FromQuery(Name = "idref")] int? idStrutturaAffiliata = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = null;
            }
            var vm = new UtenteRegistrationViewModel();
            ViewData["ReturnUrl"] = returnUrl;
            ViewData["IdAffiliato"] = idStrutturaAffiliata;
            return View(vm);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterUtente([FromForm]UtenteRegistrationViewModel model, [FromQuery] string returnUrl = null, [FromQuery(Name = "idref")] int? idStrutturaAffiliata = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl) || !Url.IsLocalUrl(returnUrl))
            {
                returnUrl = null;
            }
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {
                if (model.Password.Equals(model.PasswordConfirm))
                {
                    /* Creiamo il l'utenza*/
                    var nuovoUtente = new NuovoUtenteViewModel()
                    {
                        Cognome = model.Cognome,
                        Email = model.Email,
                        Nome = model.Nome,
                        Password = model.Password
                    };

                    var result = await _apiClient.NuovoUtenteAsync(nuovoUtente, idStrutturaAffiliata);
                    if (result)
                    {
                        return RedirectToAction("MailToConfirm");
                    }
                }
                else
                {
                    ModelState.AddModelError("Password", "Le password inserite non coincidono.");
                }
            }
            else
            {
                ModelState.AddModelError(string.Empty, "Coordinate non valide");
            }
            // If we got this far, something failed, redisplay form
            return View(model);
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
        //    return Challenge(properties, OpenIdConnectDefaults.AuthenticationScheme);
        //}


        [AllowAnonymous]
        public IActionResult MailToConfirm([FromQuery]string email, [FromQuery]string code)
        {
            return View();
        }

        //[HttpGet]
        //[AllowAnonymous]
        //[Produces("application/json")]
        //public async Task<IActionResult> CheckEmail(string email)
        //{
        //    //ViewData["ReturnUrl"] = returnUrl;
        //    bool emailIsValid = await _account.CheckEmailAsync(email);
        //    if (!emailIsValid)
        //    {
        //        return Json(data: $"L'email specificata risulta già registrata.");
        //    }
        //    else
        //    {
        //        return Json(data: emailIsValid);
        //    }
        //}


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfermaAccount(string email, string code)
        {
            try
            {
                var confirmationResult = await _apiClient.ConfermaAccount(email, WebUtility.UrlEncode(code));
                //TODO: Se si tratta di un cliente, il redirect, deve essere fatto alla home del cliente
                //      In caso di utente invece, il redirect andrebbe fatto alla struttura a cui è affiliato se presente, altrimenti alla home del sito
                if (!confirmationResult.Esito)
                {
                    return BadRequest();
                }
                var idCliente = confirmationResult.IdCliente ?? confirmationResult.IdStrutturaAffiliate;
                if (idCliente.HasValue)
                {
                    var cliente = await _apiClient.GetClienteAsync(idCliente.Value);
                    var url = Url.RouteUrl("HomeCliente", new { cliente = cliente.UrlRoute });
                    return Redirect(url);
                    //                    return RedirectToAction("HomeCliente", "Clienti", );
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