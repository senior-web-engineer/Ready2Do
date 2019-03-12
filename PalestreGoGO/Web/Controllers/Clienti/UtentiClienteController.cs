using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models;
using Web.Models.Mappers;
using Web.Models.Utils;
using Web.Proxies;
using Web.Services;
using Web.Utils;
using Web.Views.UtentiCliente;

namespace Web.Controllers.Clienti
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [Route("/{cliente}/users")]
    public class UtentiClienteController : Controller
    {
        private readonly ILogger<UtentiClienteController> _logger;
        private readonly UtentiProxy _utentiProxy;
        private readonly TipologicheProxy _tipologicheProxy;
        private readonly ClienteResolverServices _clientiResolver;

        public UtentiClienteController(ILogger<UtentiClienteController> logger,
                                         UtentiProxy utentiProxy,
                                         TipologicheProxy tipologicheProxy,
                                         ClienteResolverServices clientiResolver)
        {
            _logger = logger;
            _utentiProxy = utentiProxy;
            _clientiResolver = clientiResolver;
            _tipologicheProxy = tipologicheProxy;
        }

        [HttpGet()]
        public async Task<IActionResult> GetListaUtenti([FromRoute(Name = "cliente")]string urlRoute, [FromQuery(Name = "incstato")]bool includeStatus = true,
                                                        [FromQuery(Name = "sortby")]string sortby = "Cognome", [FromQuery(Name = "asc")]bool asc = true)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            ViewData["IdCliente"] = idCliente;
            var utenti = await _utentiProxy.GetUtentiCliente(idCliente, accessToken, includeStatus, page: 1, pageSize: 2000, sortBy: sortby, asc: asc);
            return View("ListaUtenti", utenti.ToList());
        }


        [HttpGet("{userId}")]
        public async Task<IActionResult> GetUtente([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "userId")]string userId, [FromQuery(Name = "tabId")]int tabId = 0)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
            {
                return Forbid();
            }
            ViewData["IdCliente"] = idCliente;
            ViewData["TabId"] = tabId;
            var utente = await _utentiProxy.GetUtenteCliente(idCliente, userId);
            ViewData["Utente"] = utente.MapToUserHeaderViewModel();
            var vm = utente.UtenteCliente.MapToClienteUtenteViewModel();
            var tAbb = _utentiProxy.GetAbbonamentiForUserAsync(idCliente, userId, true);
            var tCert = _utentiProxy.GetCertificatiForUserAsync(idCliente, userId, true, false);
            var tApp = _utentiProxy.GetAppuntamentiForUserAsync(idCliente, userId);
            Task.WaitAll(new Task[] { tAbb, tCert, tApp });
            vm.Abbonamenti = tAbb.Result?.MapToViewModel()?.ToList() ?? new List<AbbonamentoUtenteViewModel>();
            vm.Certificati = tCert.Result?.MapToViewModel()?.ToList() ?? new List<CertificatUtenteViewModel>();
            vm.Appuntamenti = tApp.Result?.MapToViewModel()?.ToList() ?? new List<AppuntamentoUtenteViewModel>();
            return View("DettaglioUtenteCliente", vm);
        }

        #region Gestione Abbonamenti Utenti
        ////TODO: da implementare
        //[HttpGet("{userId}/abbonamenti/add")]
        //public async Task<IActionResult> AddAbbonamentoToUser([FromRoute(Name = "cliente")]string urlRoute,
        //                                                      [FromRoute(Name = "userId")]string userId,
        //                                                      [FromQuery(Name = "tipoAbb")] int idTipoAbbonamento)
        //{
        //    int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
        //    //Verifichiamo che solo gli Admin possano accedere alla pagina di Gestione Abbonamenti
        //    if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin())
        //    {
        //        return Forbid();
        //    }
        //    var accessToken = await HttpContext.GetTokenAsync("access_token");
        //    var tipoAbbonamento = await _apiClient.GetOneTipologiaAbbonamentoAsync(idCliente, idTipoAbbonamento, accessToken);
        //    if (tipoAbbonamento == null) { return BadRequest(); }

        //    EditAbbonamentoUtenteInputModel vm = new EditAbbonamentoUtenteInputModel()
        //    {
        //        Id = null,
        //        IdCliente = idCliente,
        //        IdUtente = userId,
        //        DataInizioValidita = DateTime.Now,
        //        IdTipoAbbonamento = idTipoAbbonamento,
        //        IngressiIniziali = tipoAbbonamento.NumIngressi,
        //        IngressiResidui = tipoAbbonamento.NumIngressi,
        //        Scadenza = tipoAbbonamento.DurataMesi.HasValue ? DateTime.Now.AddMonths(tipoAbbonamento.DurataMesi.Value) : (DateTime?)null,
        //        Importo = tipoAbbonamento?.Costo ?? 0,
        //    };

        //    ViewBag.IdCliente = idCliente;
        //    ViewBag.IdUtente = userId;
        //    ViewBag.ClienteUrl = urlRoute;
        //    ViewBag.TipologiaAbbonamento = tipoAbbonamento.Nome;

        //    return View("NuovoAbbonamento", vm);
        //}

        [HttpGet("{userId}/abbonamenti/{idAbbonamento}/partial")]
        public async Task<IActionResult> PartialGetAbbonamentoForUser([FromRoute(Name = "cliente")]string urlRoute,
                                                   [FromRoute(Name = "userId")]string userId,
                                                   [FromRoute(Name = "idAbbonamento")]int idAbbonamento,
                                                   [FromQuery(Name = "mode")] string mode)
        {
            bool isEdit = (!string.IsNullOrWhiteSpace(mode) && mode.Trim().Equals("edit", StringComparison.CurrentCultureIgnoreCase));
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            var abbonamento = await _utentiProxy.GetAbbonamentoAsync(idCliente, idAbbonamento);
            if (isEdit)
            {
                return ViewComponent("UtenteEditAbbonamento");
            }
            else
            {
                return ViewComponent("UtenteViewAbbonamento");
            }
        }



        [HttpGet("{userId}/abbonamenti/{id}")]
        public async Task<IActionResult> EditAbbonamentoUtente([FromRoute(Name = "cliente")]string urlRoute,
                                                              [FromRoute(Name = "userId")]string userId,
                                                              [FromRoute(Name = "id")]int id)
        {
            //TODO: Capire se aggiungere il tipo di abbonamento in querystring e prepopolare i campi a partire dal tipo abbonamento 
            //      oppure se la scelta del tipo avviene nella view
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            var associazioneUtente = await _utentiProxy.GetUtenteCliente(idCliente, userId, new DateTime(1900, 1, 1), new DateTime(9999, 12, 31), includeAppuntamentiDaConfermare: true);
            ViewData["Utente"] = associazioneUtente.MapToUserHeaderViewModel();
            ViewData["IdCliente"] = idCliente;
            ViewData["UrlRoute"] = urlRoute;
            ViewData["HasAppuntamentiDaConfermare"] = associazioneUtente.AppuntamentiDaConfermare?.Any() ?? false;
            var tipologie = await _tipologicheProxy.GetTipologieAbbonamentiClienteAsync(idCliente);
            List<SelectListItem> items = new List<SelectListItem>();
            foreach (var t in tipologie)
            {
                items.Add(new SelectListItem(t.Nome, t.Id.ToString()));
            }
            ViewData["TipologieAbbonamenti"] = items;
            AbbonamentoUtenteViewModel vm = null;
            if (id > 0)
            {
                vm = (await _utentiProxy.GetAbbonamentoAsync(idCliente, id)).MapToViewModel();
            }
            else
            {
                vm = new AbbonamentoUtenteViewModel
                {
                    IdCliente = idCliente,
                    UserId = userId,
                    Id = -1
                };
            }
            return View("EditAbbonamento", vm);
        }


        [HttpPost("{userId}/abbonamenti/{id}")]
        public async Task<IActionResult> SaveAbbonamentoUtente([FromRoute(Name = "cliente")]string urlRoute,
                                                              [FromRoute(Name = "userId")]string userId,
                                                               [FromRoute(Name = "id")]int id,
                                                              [FromForm]AbbonamentoUtenteInputModel model)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            var associazioneUtente = await _utentiProxy.GetUtenteCliente(idCliente, userId);
            ViewData["Utente"] = associazioneUtente.MapToUserHeaderViewModel();
            ViewData["IdCliente"] = idCliente;
            ViewData["UrlRoute"] = urlRoute;
            bool modelValid = ModelState.IsValid;
            if (model.IngressiIniziali.HasValue && model.IngressiResidui.HasValue && model.IngressiResidui.Value > model.IngressiIniziali.Value)
            {
                ModelState.AddModelError("IngressiResidui", "Gli ingressi residui non possono essere maggiori degli ingressi iniziali");
                modelValid = false;
            }
            if (!modelValid)
            {
                var tipologie = await _tipologicheProxy.GetTipologieAbbonamentiClienteAsync(idCliente);
                List<SelectListItem> items = new List<SelectListItem>();
                foreach (var t in tipologie)
                {
                    items.Add(new SelectListItem(t.Nome, t.Id.ToString()));
                }
                ViewData["TipologieAbbonamenti"] = items;
                return View("EditAbbonamento", new AbbonamentoUtenteViewModel(model));
            }
            if ((model.IdCliente != idCliente) || (model.UserId != userId)) { return BadRequest(); }
            await _utentiProxy.EditAbbonamentoClienteAsync(idCliente, userId, model.MapToAPIModel());
            return RedirectToAction("GetUtente", "Clienti", new { cliente = urlRoute, userId, tabId = 0 });
        }
        #endregion

        #region CERTIFICATI
        [HttpGet("{userId}/certificati/add")]
        public async Task<IActionResult> AddCertificatoToUser([FromRoute(Name = "cliente")]string urlRoute,
                                                              [FromRoute(Name = "userId")]string userId)
        {
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            var vm = new CertificatUtenteViewModel
            {
                IdCliente = idCliente,
                UserId = userId,
            };
            return View("NuovoCertificato");
        }

        [HttpPost("{userId}/certificati/add")]
        public async Task<IActionResult> PostAddCertificatoToUser([FromRoute(Name = "cliente")]string urlRoute,
                                                              [FromRoute(Name = "userId")]string userId,
                                                              [FromForm] CertificatUtenteViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return ViewComponent(typeof(UtenteAddAbbonamentoViewComponent), model);
            }
            int idCliente = await _clientiResolver.GetIdClienteFromRouteAsync(urlRoute);
            if ((model.IdCliente != idCliente) || (model.UserId != userId)) { return BadRequest(); }
            await _utentiProxy.AddCertificatoUtenteClienteAsync(idCliente, userId, model.MapToApiModel());
            return RedirectToAction("GetUtente", "Clienti", new { cliente = urlRoute, userId, tabId = 1 });
        }


        #endregion
    }
}
