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

namespace Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly AccountServices _account;

        public AccountController(ILogger<AccountController> logger, IOptions<AppConfig> apiOptions, AccountServices account)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _account = account;
        }

        //
        // GET: /Account/Register
        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Register(string returnUrl = null)
        {
            //ViewData["ReturnUrl"] = returnUrl;
            var vm = await _account.BuildRegisterViewModelAsync(returnUrl);
            return View(vm);
        }

        //
        // POST: /Account/Register
        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegistrationInputModel model, string returnUrl = null)
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
                //NOTA: dato che usando direttamente il itpo float nem ViewModel abbiamo problemi di Culture dobbiamo parsarla a mano
                if(float.TryParse(model.Latitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out var latitudine) &&
                    float.TryParse(model.Latitudine, NumberStyles.Float, CultureInfo.InvariantCulture, out var longitudine))
                {
                    apiModel.Coordinate = new CoordinateViewModel(latitudine, longitudine);
                    var result = await WebAPIClient.NuovoClienteAsync(apiModel, _appConfig.WebAPI.BaseAddress);
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
            return View(await _account.BuildRegisterViewModelAsync(model));
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult MailToConfirm()
        {
            return View();
        }


        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfermaAccount(string email, string code)
        {
            try
            {
                var url = await WebAPIClient.ConfermaAccount(email, WebUtility.UrlEncode(code), _appConfig.WebAPI.BaseAddress);

                return RedirectToAction("Index", "Clienti", url);
            }
            catch
            {
                return BadRequest();
            }
        }
    }
}