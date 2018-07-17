using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Web.Configuration;
using Web.Models;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class UsersController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;
        private readonly ClienteResolverServices _clientiResolver;

        public UsersController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 WebAPIClient apiClient,
                                 ClienteResolverServices clientiResolver)
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
            _clientiResolver = clientiResolver;
        }

        //Ritorna il profilo dell'utente
        [HttpGet()]
        public async Task<IActionResult> Index([FromRoute(Name = "cliente")]string urlRoute)
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.UserId();
            if (!userId.HasValue)
            {
                return Forbid();
            }
            var items = await _apiClient.GetAppuntamentiForCurrentUserAsync(userId.Value, accessToken);
            var vm = new UserProfileViewModel();
            var appuntamenti = new List<AppuntamentoUtenteViewModel>();
            vm.Appuntamenti = appuntamenti;
            AppuntamentoUtenteViewModel appuntamento;
            foreach (var i in items)
            {
                appuntamento = new AppuntamentoUtenteViewModel()
                {
                    Id = i.IdAppuntamento,
                    DataCancellazione = i.DataOraCancellazione,
                    DataOra = i.DataOra,
                    DataOraIscrizione = i.DataOraIscrizione,
                    Nome = i.Nome,
                    NomeCliente = i.NomeCliente,
                    Cancellabile = i.DataOra > DateTime.Now.AddMinutes(30)
                };
                appuntamenti.Add(appuntamento);
            }
            return View(vm);
        }
    }
}