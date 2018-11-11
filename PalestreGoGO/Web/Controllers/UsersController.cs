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
        public async Task<IActionResult> Index()
        {
            string accessToken = await HttpContext.GetTokenAsync("access_token");
            var userId = User.UserId();
            if (string.IsNullOrWhiteSpace(userId))
            {
                return Forbid();
            }
            var items = await _apiClient.GetAppuntamentiForCurrentUserAsync(userId, accessToken);
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
                    ClienteUrlRoute = await _clientiResolver.GetRouteClienteFromIdAsync(i.IdCliente),
                    DataOra = i.DataOra,
                    DataOraIscrizione = i.DataOraIscrizione,
                    Nome = i.Nome,
                    NomeCliente = i.NomeCliente,
                    Cancellabile = i.DataOra > DateTime.Now.AddMinutes(30),
                    IdEvento = i.IdEvento
                };
                appuntamenti.Add(appuntamento);
            }
            return View(vm);
        }

        #region Gestione Associazione Utenti

        /// <summary>
        /// Associa l'utente corrente alla struttura (cliente)
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <returns></returns>
        [HttpPost("associa/{idCliente:int}")]
        public async Task<IActionResult> AssociaToCliente([FromRoute]int idCliente, [FromQuery(Name = "returnUrl")]string returnUrl)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken)) { return Forbid(); }
//            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _apiClient.ClienteFollowAsync(idCliente, accessToken);
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                //Possibile problema di sicurezza? (Open Redirect?)
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", new { idCliente = idCliente});
            }
        }

        /// <summary>
        /// Associa l'utente corrente alla struttura (cliente)
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <returns></returns>
        [HttpPost("disassocia/{idCliente:int}")]
        public async Task<IActionResult> RemoveAssociazioneToCliente([FromRoute]int idCliente, [FromQuery(Name = "returnUrl")]string returnUrl)
        {
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            if (string.IsNullOrEmpty(accessToken)) { return Forbid(); }
  //          int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _apiClient.ClienteUnFollowAsync(idCliente, accessToken);
            if (!string.IsNullOrWhiteSpace(returnUrl))
            {
                //Possibile problema di sicurezza? (Open Redirect?)
                return Redirect(returnUrl);
            }
            else
            {
                return RedirectToAction("Index", new { idCliente = idCliente });
            }
        }
        #endregion

    }
}