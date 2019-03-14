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
            if (User.Identity.IsAuthenticated)
            {
                Task[] tasks = new Task[4];
                tasks[0]= Task.Run(async() => { vm.Schedule = await _schedulesProxy.GetScheduleAsync(idCliente, idEvento); });
                tasks[1] = Task.Run(async () => { vm.Appuntamenti = await _schedulesProxy.GetAppuntamentiForEventoAsync(idCliente, idEvento); });
                tasks[2] = Task.Run(async () => { vm.AppuntamentiDaConfermare = await _schedulesProxy.GetAppuntamentiDaConferamareForEventoAsync(idCliente, idEvento); });
                tasks[3] = Task.Run(async () => { vm.WaitListRegistrations = await _schedulesProxy.GetWaitListRegistrationsForEventoAsync(idCliente, idEvento); });
                Task.WaitAll(tasks);
                //vm.Schedule = tasks[0].r //await _schedulesProxy.GetScheduleAsync(idCliente, idEvento);
                //vm.Appuntamenti = await _schedulesProxy.GetAppuntamentiForEventoAsync(idCliente, idEvento);
                //vm.AppuntamentiDaConfermare = await _schedulesProxy.GetAppuntamentiDaConferamareForEventoAsync(idCliente, idEvento);
                //vm.WaitListRegistrations = await _schedulesProxy.GetWaitListRegistrationsForEventoAsync(idCliente, idEvento);
                ViewData["UserHasAppointment"] = (vm.Appuntamenti?.Any() ?? false) || (vm.AppuntamentiDaConfermare?.Any() ?? false);
                ViewData["UserInWaitList"] = vm.WaitListRegistrations?.Any() ?? false;
            }
            else
            {
                vm.Schedule = await _schedulesProxy.GetScheduleAsync(idCliente, idEvento);
            }
            return View("DettaglioEvento", vm);
        }

      
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
            return RedirectToAction("DettaglioEvento", new { cliente = urlRoute, id = idEvento });
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
            return RedirectToAction("DettaglioEvento", new { cliente=urlRoute, id=idEvento });
        }


        /// <summary>
        /// Questa action è riservata ai gestori che vogliono confermare un AppuntamentoDaConfermare, dopo che hanno 
        /// associato un abbonamento all'utente
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <param name="idEvento"></param>
        /// <returns></returns>
        [HttpPost("{cliente}/eventi/{idEvento:int}/appuntamento/{idAppuntamento:int}/conferma")]
        public async Task<IActionResult> ConfermaAppuntamento([FromRoute(Name = "cliente")] string urlRoute,
                                                              [FromRoute(Name = "idEvento")]int idEvento,
                                                              [FromRoute(Name = "idAppuntamento")]int idAppuntamento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _schedulesProxy.ConfermaAppuntamentoAsyn(idCliente, idEvento, idAppuntamento);
            return RedirectToAction("DettaglioEvento", new { cliente = urlRoute, id = idEvento });
        }

        /// <summary>
        /// Questa action è riservata ai gestori che vogliono rifiutare un AppuntamentoDaConfermare
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <param name="idEvento"></param>
        /// <param name="idAppuntamento"></param>
        /// <returns></returns>
        [HttpPost("{cliente}/eventi/{idEvento:int}/appuntamento/{idAppuntamento:int}/rifiuta")]
        public async Task<IActionResult> RifiutaAppuntamento([FromRoute(Name = "cliente")] string urlRoute,
                                                              [FromRoute(Name = "idEvento")]int idEvento,
                                                              [FromRoute(Name = "idAppuntamento")]int idAppuntamento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            await _schedulesProxy.RifiutaAppuntamentoAsyn(idCliente, idEvento, idAppuntamento);
            return RedirectToAction("DettaglioEvento", new { cliente = urlRoute, id = idEvento });
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
