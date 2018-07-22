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
using System.Globalization;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.OpenIdConnect;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class EventiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;
        private readonly WebAPIClient _apiClient;
        private readonly ClienteResolverServices _clientsResolver;

        public EventiController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions,
                                 WebAPIClient apiClient,
                                 ClienteResolverServices clientsResolver
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
            _apiClient = apiClient;
            _clientsResolver = clientsResolver;
        }


        [HttpGet("{cliente}/eventi/new")]
        public async Task<IActionResult> NewEvento([FromRoute(Name = "cliente")]string urlRoute,
                                                    [FromQuery(Name = "lid")] string lid,
                                                    [FromQuery(Name = "date")] string dataEvento,
                                                    [FromQuery(Name = "time")] string oraEvento)
        {
            var vm = new EventoViewModel();
            DateTime dataParsed;
            TimeSpan timeParsed;
            int idLocation;
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            var tipoLezioni = await _apiClient.GetTipologieLezioniClienteAsync(idCliente);
            var locations = await _apiClient.GetLocationsAsync(idCliente);
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

        [HttpPost("{cliente}/eventi/new")]
        [HttpPost("{cliente}/eventi/edit/{id}")]
        public async Task<IActionResult> SaveEvento([FromRoute(Name = "cliente")]string urlRoute, [FromForm] EventoViewModel evento, [FromRoute(Name = "id")] int? idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            var tipoLezioni = await _apiClient.GetTipologieLezioniClienteAsync(idCliente);
            var locations = await _apiClient.GetLocationsAsync(idCliente);
            if (!ModelState.IsValid)
            {
                ViewData["TipologieLezioni"] = new SelectList(tipoLezioni, "Id", "Nome");
                ViewData["Locations"] = new SelectList(locations, "Id", "Nome");
                ViewData["IdCliente"] = idCliente;
                return View("EditEvento", evento);
            }

            if((evento.DataCancellazioneMax.HasValue && !evento.OraCancellazioneMax.HasValue) ||
                (evento.DataCancellazioneMax.HasValue && !evento.OraCancellazioneMax.HasValue))
            {
                ModelState.AddModelError("DataOraCancellazione", "E' necessario specificare sia la Data che l'Ora limite per la Cancellazione dell'evento");
                return View("EditEvento", evento);
            }

            if (evento.DataCancellazioneMax.HasValue)
            {
                var dtCanc = evento.DataCancellazioneMax.Value.Add(evento.OraCancellazioneMax.Value);
                var dtEvento = evento.Data.Value.Add(evento.OraInizio.Value);
                if(dtEvento < dtCanc)
                {
                    ModelState.AddModelError("DataOraCancellazioneTooLate", "La Data limite per la Cancellazione dell'evento deve essere anteriore alla data dell'evento stesso.");
                    return View("EditEvento", evento);
                }
            }

                //TODO: Verificare che la data cancellazione sia antecedente la data evento e le eventuali
                ScheduleViewModel apiVM = new ScheduleViewModel()
            {
                CancellabileFinoAl = evento.DataCancellazioneMax.Value.Add(evento.OraCancellazioneMax.Value),
                Data = evento.Data.Value,
                Title = evento.Title,
                IdCliente = idCliente,
                IdLocation = evento.IdLocation.Value,
                Istruttore = evento.Istruttore,
                Note = evento.Note,
                OraInizio = evento.OraInizio.Value,
                PostiDisponibili = evento.PostiDisponibili,
                IdTipoLezione = evento.IdTipoLezione.Value,
                Id = evento.Id
            };
            var accessToken = await HttpContext.GetTokenAsync("access_token");
            await _apiClient.SaveSchedule(idCliente, apiVM, accessToken);
            return RedirectToAction("Index", "Schedules");
        }


        [HttpGet("{cliente}/eventi/edit/{id}")]
        public async Task<IActionResult> EditEvento([FromRoute(Name = "cliente")]string urlRoute,
                                                    [FromRoute(Name = "id")] int idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            //var cliente = await _apiClient.GetClienteAsync(urlRoute);
            var tipoLezioni = await _apiClient.GetTipologieLezioniClienteAsync(idCliente);
            var locations = await _apiClient.GetLocationsAsync(idCliente);
            var evento = await _apiClient.GetScheduleAsync(idCliente, idEvento);
            ViewData["TipologieLezioni"] = new SelectList(tipoLezioni, "Id", "Nome");
            ViewData["Locations"] = new SelectList(locations, "Id", "Nome");
            ViewData["IdCliente"] = idCliente;
            return View("EditEvento", internalBuildViewModel(evento));
        }

        /// <summary>
        /// Se l'utente non è autenticato vedrà solo i dettagli dell'evento senza poter creare/modificare l'appuntamento
        /// Se invece l'utente è autenticato può creare un nuovo appuntamento o annullarne uno precedentemente preso.
        /// Come verifichiamo che l'utente sia "associato" alla struttura?
        /// </summary>
        /// <param name="urlRoute"></param>
        /// <param name="idEvento"></param>
        /// <returns></returns>
        [HttpGet("{cliente}/eventi/{idEvento}/appuntamento")]
        [AllowAnonymous]
        public async Task<IActionResult> GetAppuntamentoEvento([FromRoute(Name = "cliente")] string urlRoute, [FromRoute(Name = "idEvento")]int idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            var schedule = await _apiClient.GetScheduleAsync(idCliente, idEvento);
            string accessToken = null;
            if (User.Identity.IsAuthenticated)
            {
                accessToken = await HttpContext.GetTokenAsync("access_token");
            }
            ViewBag.IdCliente = idCliente;
            ViewBag.Cliente = urlRoute;
            ViewBag.IdEvento = idEvento;
            var appuntamento = await _apiClient.GetAppuntamentoForCurrentUserAsync(idCliente, idEvento, accessToken);
            return View("Appuntamento", appuntamento);
        }

        [HttpPost("{cliente}/eventi/{idEvento}/appuntamento")]
        public async Task<IActionResult> AddAppuntamentoEvento([FromRoute(Name = "cliente")] string urlRoute, [FromRoute(Name = "idEvento")]int idEvento)
        {
            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            var access_token = await HttpContext.GetTokenAsync("access_token");
            var apiModel = new NuovoAppuntamentoApiModel()
            {
                IdEvento = idEvento,
                IdUtente = User.UserId().Value
            };
            await _apiClient.SalvaAppuntamentoForCurrentUser(idCliente, apiModel, access_token);
            return RedirectToAction("GetAppuntamentoEvento", new { cliente = urlRoute, idEvento = idEvento });
        }


        [HttpPost("{cliente}/eventi/{idEvento:int}/appuntamento/{idAppuntamento:int}/delete")]
        public async Task<IActionResult> DeleteAppuntamento([FromRoute(Name = "cliente")] string urlRoute, 
                                                            [FromRoute(Name = "idEvento")]int idEvento, 
                                                            [FromRoute(Name = "idAppuntamento")]int idAppuntamento,
                                                            [FromQuery(Name ="returnUrl")] string returnUrl)
        {
            //NOTA: idEvento non utilizzato
            if (string.IsNullOrWhiteSpace(returnUrl)) { return BadRequest(); }

            var idCliente = await _clientsResolver.GetIdClienteFromRouteAsync(urlRoute);
            var access_token = await HttpContext.GetTokenAsync("access_token");

            await _apiClient.DeleteAppuntamentoAsync(idCliente, idAppuntamento, access_token);
            return Redirect(returnUrl);
            //return RedirectToAction("GetAppuntamentoEvento", new { cliente = idCliente, idEvento = idEvento });
        }


        private EventoViewModel internalBuildViewModel(ScheduleDetailsViewModel apiModel)
        {
            EventoViewModel vm = new EventoViewModel()
            {
                DataCancellazioneMax = apiModel.CancellabileFinoAl.Date,
                OraCancellazioneMax = apiModel.CancellabileFinoAl.TimeOfDay,
                Title = apiModel.Title,
                Data = apiModel.Data,
                IdLocation = apiModel.IdLocation,
                IdTipoLezione = apiModel.IdTipoLezione,
                Istruttore = apiModel.Istruttore,
                Note = apiModel.Note,
                OraInizio = apiModel.OraInizio,
                PostiDisponibili = apiModel.PostiDisponibili,
                Id = apiModel.Id
            };
            return vm;
        }
    }
}
