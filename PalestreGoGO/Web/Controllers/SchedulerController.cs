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
using Web.Models.Utils;
using Web.Models.Mappers;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    [Route("{cliente}/schedules")]
    public class SchedulerController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;
        private readonly ClienteResolverServices _clientiResolver;

        public SchedulerController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 WebAPIClient apiClient,
                                 ClienteResolverServices clientiResolver
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
            _clientiResolver = clientiResolver;
        }

        [HttpGet]
        public async Task<IActionResult> Index([FromRoute(Name = "cliente")]string urlRoute, [FromQuery(Name = "lid")] int? idActiveLocation)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return RedirectToAction("Index", "Clienti", new { cliente = urlRoute });
            }
            var cliente = await _apiClient.GetClienteAsync(urlRoute);
            ViewData["IdCliente"] = idCliente;
            ViewData["AuthToken"] = GenerateAuthenticationToken(urlRoute, idCliente);
            var vm = new SchedulerViewModel();
            vm.Sale = await _apiClient.GetLocationsAsync(idCliente);
            vm.IdActiveLocation = idActiveLocation ?? vm.Sale.FirstOrDefault()?.Id;

            cliente.OrarioApertura.MapOrarioApertura().GetMinMax(out TimeSpan? min, out TimeSpan? max);
            vm.MinTime = min;
            vm.MaxTime = max;
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
