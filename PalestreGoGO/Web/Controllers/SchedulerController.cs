using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using PalestreGoGo.WebAPIModel;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Web.Utils;
using Web.Models;
using Web.Services;
using Web.Configuration;
using Newtonsoft.Json;
using System.Text;

namespace Web.Controllers
{
    //[Authorize]
    [Route("{cliente}/schedules")]
    public class SchedulerController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;

        public SchedulerController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute(Name = "cliente")]string urlRoute, [FromQuery(Name = "lid")] int? idActiveLocation)
        {
            var cliente = await WebAPIClient.GetClienteAsync(urlRoute, _appConfig.WebAPI.BaseAddress);
            ViewData["AuthToken"] = GenerateAuthenticationToken(urlRoute, cliente.IdCliente);
            var vm = new SchedulerViewModel();
            vm.Sale = await WebAPIClient.GetLocationsAsync(cliente.IdCliente, _appConfig.WebAPI.BaseAddress);
            vm.IdActiveLocation = idActiveLocation ?? vm.Sale.First().Id;
            return View(vm);
        }

        #region Helpers
        /// <summary>
        /// Genera una stringa rappresentante un "token" per l'autenticazione delle chiamate Ajax
        /// </summary>
        /// <param name="securityToken"></param>
        /// <param name="storageContainer"></param>
        /// <returns></returns>
        private string GenerateAuthenticationToken(string clientRouteUrl, int idCliente)
        {
            var token = new AuthTokenModel()
            {
                ClientRoute = clientRouteUrl,
                CreationTime = DateTime.Now,
                IdCliente = idCliente
            };
            string json = JsonConvert.SerializeObject(token, Formatting.None);
            //Cifriamo il json ottenuto
            var result = SecurityUtils.EncryptStringWithAes(json, Encoding.UTF8.GetBytes(_appConfig.EncryptKey));
            return result;
        }
        #endregion
    }
}
