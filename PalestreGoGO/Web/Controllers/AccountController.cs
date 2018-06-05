using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
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
using Microsoft.AspNetCore.WebUtilities;
using System.Net;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly AccountServices _account;

        private readonly WebAPIClient _apiClient;

        public AccountController(ILogger<AccountController> logger, IOptions<AppConfig> apiOptions, AccountServices account, WebAPIClient apiClient)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _account = account;
            _apiClient = apiClient;
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = Url.Action("Index", "Home");
            }
            var authProps = new AuthenticationProperties()
            {
                RedirectUri = returnUrl
            };
            return Challenge(authProps, OpenIdConnectDefaults.AuthenticationScheme);
        }

        [HttpGet]
        public IActionResult Logout(string returnUrl = null)
        {
            if (string.IsNullOrWhiteSpace(returnUrl))
            {
                returnUrl = this.Url.Action("Index", "Home");
            }
            //NOTA: Aggiungere le properties?
            AuthenticationProperties props = new AuthenticationProperties()
            {
                RedirectUri = returnUrl
            };
            return SignOut(OpenIdConnectDefaults.AuthenticationScheme);
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RegisterCliente(string returnUrl = null)
        {
            //ViewData["ReturnUrl"] = returnUrl;
            var vm = await _account.BuildRegisterClienteViewModelAsync(returnUrl);
            return View(vm);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RegisterCliente(ClientRegistrationInputModel model, string returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            if (ModelState.IsValid)
            {

                /* Creiamo il l'utenza*/
                var apiModel = new NuovoClienteViewModel()
                {
                    Citta = model.Citta,
                    ZipOrPostalCode = model.CAP,
                    Country = model.Country,
                    Email = model.Email,
                    IdTipologia = model.IdTipologia,
                    Indirizzo = model.Indirizzo,
                    Nome = model.Nome,
                    Cognome = model.Cognome,
                    NumTelefono = model.Telefono,
                    RagioneSociale = model.RagioneSociale,
                    UrlRoute = model.URL,
                    NuovoUtente = new NuovoUtenteViewModel()
                    {
                        Cognome = model.Cognome,
                        Email = model.Email,
                        Nome = model.Nome,
                        Password = model.Password,
                        Telefono = model.Telefono
                    }
                };
                //Parsing coordinate
                //NOTA: dato che usando direttamente il tipo float nel ViewModel abbiamo problemi di Culture dobbiamo parsarla a mano
                if (float.TryParse(model.Latitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitudine) &&
                    float.TryParse(model.Longitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitudine))
                {
                    apiModel.Coordinate = new CoordinateViewModel(latitudine, longitudine);
                    var result = await _apiClient.NuovoClienteAsync(apiModel);
                    if (result)
                    {
                        return RedirectToAction("MailToConfirm");
                    }
                    else
                    {
                        //Dobbiamo gestire meglio gli errori lato API!
                        ModelState.AddModelError(string.Empty, "Errore durante la creazione del cliente");
                    }
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Coordinate non valide");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(await _account.BuildRegisterClienteViewModelAsync(model));
        }


        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public IActionResult RegisterUtente([FromQuery] string returnUrl = null, [FromQuery(Name = "idref")] int? idStrutturaAffiliata = null)
        {
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


        [AllowAnonymous]
        public IActionResult MailToConfirm([FromQuery]string email, [FromQuery]string code)
        {
            return View();
        }

        [HttpGet]
        [AllowAnonymous]
        [Produces("application/json")]
        public async Task<IActionResult> CheckEmail(string email)
        {
            //ViewData["ReturnUrl"] = returnUrl;
            bool emailIsValid = await _account.CheckEmailAsync(email);
            if (!emailIsValid)
            {
                return Json(data: $"L'email specificata risulta già registrata.");
            }
            else
            {
                return Json(data: emailIsValid);
            }
        }


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