using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Models;
using Web.Configuration;
using Newtonsoft.Json;
using System.Text;
using Web.Models.Utils;

namespace Web.Controllers
{
    //[Authorize]
    public class ClientiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;

        public ClientiController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> Index([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await WebAPIClient.GetClienteAsync(urlRoute, _appConfig.WebAPI.BaseAddress);
            return View(cliente.MapToHomeViewModel());
        }

        public IActionResult CalendarEdit(/*[FromRoute(Name ="id")]int idCliente*/ [FromRoute(Name = "cliente")]string cliente)
        {
            //var cliente = WebAPIClient.GetClienteAsync()
            return View();
        }

        [HttpGet]
        public async Task<IActionResult> ProfileEdit([FromRoute(Name = "cliente")]string urlRoute)
        {
            var cliente = await WebAPIClient.GetClienteAsync(urlRoute, _appConfig.WebAPI.BaseAddress);
            var vm = new ClienteProfileEditViewModel();
            vm.SASToken = this.GenerateAuthenticationToken(cliente.SecurityToken, cliente.StorageContainer);
            if (cliente.Immagini != null)
            {
                foreach (var img in cliente.Immagini)
                {
                    vm.GalleryImages.Add(new ImageViewModel()
                    {
                        Id = img.Id,
                        Alt = img.Alt,
                        Caption = img.Nome,
                        Url = img.Url
                    });
                }
            }
            //TODO: Completare popolamento VM
            return View(vm);
        }


        #region Helpers
        /// <summary>
        /// Genera una stringa rappresentante un "token" per l'autenticazione delle chiamate Ajax
        /// </summary>
        /// <param name="secuirtyToken"></param>
        /// <param name="storageContainer"></param>
        /// <returns></returns>
        private string GenerateAuthenticationToken(string secuirtyToken, string storageContainer)
        {
            var token = new SASTokenModel()
            {
                SecurityToken = secuirtyToken,
                ContainerName = storageContainer,
                CreationTime = DateTime.Now
            };
            string json = JsonConvert.SerializeObject(token, Formatting.None);
            //Cifriamo il json ottenuto
            return SecurityUtils.EncryptStringWithAes(json, Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
        }
        #endregion
    }
}