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
                    Coordinate = model.Latitudine.HasValue && model.Longitudine.HasValue ?  new CoordinateViewModel(model.Latitudine.Value, model.Longitudine.Value) : null,
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
                //TODO: Convertire da VM a APIModel
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

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        [HttpGet]
        [AllowAnonymous]
        public IActionResult MailToConfirm()
        {
            return View();
        }

        ////
        //// POST: /Account/LogOff
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> LogOff()
        //{
        //    await _signInManager.SignOutAsync();
        //    _logger.LogInformation(4, "User logged out.");
        //    return RedirectToAction(nameof(HomeController.Index), "Home");
        //}

    }
}