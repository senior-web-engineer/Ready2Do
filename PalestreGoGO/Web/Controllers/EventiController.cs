using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using Serilog;
using System;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Web.Configuration;
using Web.Models;
using Web.Models.Mappers;
using Web.Proxies;
using Web.Services;
using Web.Utils;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    public class EventiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly TipologicheProxy _tipologicheProxy;
        private readonly SchedulesProxy _schedulesProxy;
        private readonly ClienteResolverServices _clientsResolver;

        public EventiController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 TipologicheProxy tipologicheProxy,
                                 SchedulesProxy schedulesProxy,
                                 ClienteResolverServices clientsResolver
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _tipologicheProxy = tipologicheProxy;
            _clientsResolver = clientsResolver;
            _schedulesProxy = schedulesProxy;
        }


        [HttpGet("{cliente}/eventi/new")]
        public async Task<IActionResult> NewEvento([FromRoute(Name = "cliente")]string urlRoute,
                                                    [FromQuery(Name = "lid")] string lid,
                                                    [FromQuery(Name = "date")] string dataEvento,
                                                    [FromQuery(Name = "time")] string oraEvento)
        {
            var vm = new ScheduleEditViewModel();
            DateTime dataParsed;
            TimeSpan timeParsed;
            int idLocation;
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            var tipoLezioni = await _tipologicheProxy.GetTipologieLezioniClienteAsync(idCliente);
            var locations = await _tipologicheProxy.GetLocationsAsync(idCliente);
            if (!string.IsNullOrWhiteSpace(dataEvento) && DateTime.TryParseExact(dataEvento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dataParsed))
            {
                vm.Data = dataParsed;
                vm.DataCancellazioneMax = dataParsed.AddDays(-1);
            }
            if (!string.IsNullOrWhiteSpace(oraEvento) && TimeSpan.TryParseExact(oraEvento, "c", CultureInfo.InvariantCulture, out timeParsed))
            {
                vm.OraInizio = timeParsed;
                vm.OraCancellazioneMax = timeParsed;
            }
            if (!string.IsNullOrWhiteSpace(lid) && int.TryParse(lid, out idLocation))
            {
                vm.IdLocation = idLocation;
            }
            ViewData["TipologieLezioni"] = new SelectList(tipoLezioni, "Id", "Nome");
            ViewData["Locations"] = new SelectList(locations, "Id", "Nome");
            ViewData["IdCliente"] = idCliente;
            return View("EditEvento", vm);
        }

        [HttpGet("{cliente}/eventi/edit/{id}")]
        public async Task<IActionResult> ModificaEvento([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name ="id")] int idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin()) { return Forbid(); }
            var evento = await _schedulesProxy.GetScheduleAsync(idCliente, idEvento);
            var tipoLezioni = await _tipologicheProxy.GetTipologieLezioniClienteAsync(idCliente);
            var locations = await _tipologicheProxy.GetLocationsAsync(idCliente);
            ViewData["TipologieLezioni"] = new SelectList(tipoLezioni, "Id", "Nome");
            ViewData["Locations"] = new SelectList(locations, "Id", "Nome");
            ViewData["IdCliente"] = idCliente;
            ViewData["IdEvento"] = idEvento;
            return View("EditEvento", new ScheduleEditViewModel(evento));

        }


        [HttpPost("{cliente}/eventi/new")]
        [HttpPost("{cliente}/eventi/edit/{id}")]
        public async Task<IActionResult> SaveEvento([FromRoute(Name = "cliente")]string urlRoute, [FromForm] ScheduleInputViewModel evento, [FromRoute(Name = "id")] int? idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            if (!ModelState.IsValid)
            {
                var tipoLezioni = await _tipologicheProxy.GetTipologieLezioniClienteAsync(idCliente);
                var locations = await _tipologicheProxy.GetLocationsAsync(idCliente);
                ViewData["TipologieLezioni"] = new SelectList(tipoLezioni, "Id", "Nome");
                ViewData["Locations"] = new SelectList(locations, "Id", "Nome");
                ViewData["IdCliente"] = idCliente;
                return View("EditEvento", new ScheduleEditViewModel(evento));
            }

            await _schedulesProxy.SaveSchedule(idCliente, evento.ToApiModel(idCliente));
            return RedirectToAction("Index", "Scheduler", new { cliente = urlRoute, lid = evento.IdLocation });
        }


        [HttpGet("{cliente}/eventi/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> DettaglioEvento([FromRoute(Name = "cliente")]string urlRoute,
                                                    [FromRoute(Name = "id")] int idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            ViewData["ClienteRoute"] = urlRoute;
            ViewData["IdCliente"] = idCliente;
            ViewData["IdEvento"] = idEvento;
            ViewData["UserType"] = User.GetUserTypeForCliente(idCliente);
            var vm = new DettaglioEventoViewModel();
            vm.Schedule = await _schedulesProxy.GetScheduleAsync(idCliente, idEvento);
            if (User.Identity.IsAuthenticated)
            {
                vm.Appuntamenti = await _schedulesProxy.GetAppuntamentiForEventoAsync(idCliente, idEvento);
                vm.AppuntamentiDaConfermare = await _schedulesProxy.GetAppuntamentiDaConferamareForEventoAsync(idCliente, idEvento);
                vm.WaitListRegistrations = await _schedulesProxy.GetWaitListRegistrationsForEventoAsync(idCliente, idEvento);
                ViewData["UserHasAppointment"] = (vm.Appuntamenti?.Any() ?? false) || (vm.AppuntamentiDaConfermare?.Any() ?? false);
                ViewData["UserInWaitList"] = vm.WaitListRegistrations?.Any() ?? false;
            }
            return View("DettaglioEvento", vm);
        }

        ///// <summary>
        ///// Action che ritorna i dati di un evento in visualizzazione.
        ///// In base alla tipologia di utente chiamante saranno abilitate o meno le funzioni di registrazioni o di amministrazione
        ///// </summary>
        ///// <param name="urlRoute"></param>
        ///// <param name="idEvento"></param>
        ///// <returns></returns>
        //[HttpGet("{cliente}/eventi/{id}")]
        //[AllowAnonymous] //Visibile anche dagli utenti anonimi
        //public async Task<IActionResult> ViewEvento([FromRoute(Name = "cliente")]string urlRoute, [FromRoute(Name = "id")] int idEvento)
        //{
        //    var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
        //    var userType = User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin();
        //    ViewData["UserType"] = userType;

        //    return View("ViewEvento");
        //}





        ///// <summary>
        ///// Se l'utente non è autenticato vedrà solo i dettagli dell'evento senza poter creare/modificare l'appuntamento
        ///// Se invece l'utente è autenticato può creare un nuovo appuntamento o annullarne uno precedentemente preso.
        ///// Come verifichiamo che l'utente sia "associato" alla struttura?
        ///// </summary>
        ///// <param name="urlRoute"></param>
        ///// <param name="idEvento"></param>
        ///// <returns></returns>
        //[HttpGet("{cliente}/eventi/{idEvento}/appuntamento")]
        //[AllowAnonymous]
        //public async Task<IActionResult> GetAppuntamentoEvento([FromRoute(Name = "cliente")] string urlRoute, [FromRoute(Name = "idEvento")]int idEvento)
        //{
        //    var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
        //    var schedule = await _apiClient.GetScheduleAsync(idCliente, idEvento);
        //    string accessToken = null;
        //    if (User.Identity.IsAuthenticated)
        //    {
        //        accessToken = await HttpContext.GetTokenAsync("access_token");
        //    }
        //    ViewBag.IdCliente = idCliente;
        //    ViewBag.Cliente = urlRoute;
        //    ViewBag.IdEvento = idEvento;
        //    //TODO: Modificare l'API invocata usando quella specifica del cliente per verificare lo stato
        //    //var followInfo = (await _apiClient.ClientiFollowedByUserAsync(User.UserId(), accessToken)).FirstOrDefault(cf => cf.IdCliente.Equals(idCliente));
        //    throw new NotImplementedException();
        //    /*ViewBag.ClienteFollowed = followInfo != null;
        //    ViewBag.AbbonamentoValido = followInfo?.HasAbbonamentoValido ?? false;
        //    var appuntamento = await _apiClient.GetAppuntamentoForCurrentUserAsync(idCliente, idEvento, accessToken);
        //    return View("Appuntamento", appuntamento);
        //    */
        //}

        #region APPUNTAMENTI PER EVENTO
        [HttpPost("{cliente}/eventi/{idEvento}/appuntamento")]
        public async Task<IActionResult> TakeAppuntamento([FromRoute(Name = "cliente")] string urlRoute, [FromRoute(Name = "idEvento")]int idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            var apiModel = new NuovoAppuntamentoApiModel()
            {
                IdEvento = idEvento,
                IdUtente = User.UserId()
            };
            await _schedulesProxy.TakeAppuntamentoForCurrentUser(idCliente, apiModel);
            return RedirectToAction("GetAppuntamentoEvento", new { cliente = urlRoute, idEvento = idEvento });
        }


        /// <summary>
        /// Questa Action è destinata agli utenti ordinari che vogliono cancellare il proprio Appuntamento per l'evento.
        /// Non viene specificato l'idAppuntamento perché ce ne può essere solo uno per l'utente corrente
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <param name="idEvento"></param>
        /// <param name="returnUrl"></param>
        /// <returns></returns>
        [HttpPost("{cliente}/eventi/{idEvento:int}/appuntamento/delete")]
        public async Task<IActionResult> DeleteAppuntamento([FromRoute(Name = "cliente")] string urlRoute,
                                                            [FromRoute(Name = "idEvento")]int idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _schedulesProxy.DeleteAppuntamentoUserAsync(idCliente, idEvento);
            return RedirectToAction("DettaglioEvento", new { cliente=urlRoute, idEvento });
        }


        private ScheduleEditViewModel internalBuildViewModel(ScheduleDM apiModel)
        {
            ScheduleEditViewModel vm = new ScheduleEditViewModel()
            {
                DataCancellazioneMax = apiModel.CancellabileFinoAl?.Date,
                OraCancellazioneMax = apiModel.CancellabileFinoAl?.TimeOfDay,
                Title = apiModel.Title,
                Data = apiModel.DataOraInizio.Date,
                IdLocation = apiModel.IdLocation,
                IdTipoLezione = apiModel.IdTipoLezione,
                Istruttore = apiModel.Istruttore,
                Note = apiModel.Note,
                OraInizio = apiModel.DataOraInizio.TimeOfDay,
                PostiDisponibili = apiModel.PostiDisponibili,
                Id = apiModel.Id,
                DataChiusuraIscrizioni = apiModel.DataChiusuraIscrizione?.Date,
                OraChiusuraIscrizioni = apiModel.DataChiusuraIscrizione?.TimeOfDay
            };
            return vm;
        }
        #endregion
    }
}
