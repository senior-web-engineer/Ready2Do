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

namespace Web.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly IConfiguration _configuration;
        private readonly WebAPIConfig _apiOptions;
        private readonly AccountServices _account;

        public AccountController(ILogger<AccountController> logger, IConfiguration configuration, IOptions<WebAPIConfig> apiOptions, AccountServices account)
        {
            _logger = logger;
            _configuration = configuration;
            _apiOptions = apiOptions.Value;
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
                var apiModel = new NuovoClienteViewModel();
                //TODO: Convertire da VM a APIModel
                var result = await WebAPIClient.NuovoClienteAsync(apiModel, _apiOptions.BaseAddress);
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