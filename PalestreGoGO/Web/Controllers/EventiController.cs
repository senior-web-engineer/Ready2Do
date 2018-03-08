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

namespace Web.Controllers
{
    //[Authorize]
    public class EventiController : Controller
    {
        private readonly ILogger<AccountController> _logger;
        private readonly AppConfig _appConfig;

        public EventiController(ILogger<AccountController> logger,
                                 IOptions<AppConfig> apiOptions
                            )
        {
            _logger = logger;
            _appConfig = apiOptions.Value;
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
            var cliente = await WebAPIClient.GetClienteAsync(urlRoute, _appConfig.WebAPI.BaseAddress);
            var tipoLezioni = await WebAPIClient.GetTipologieLezioniClienteAsync(_appConfig.WebAPI.BaseAddress, cliente.IdCliente);
            var locations = await WebAPIClient.GetLocationsAsync(cliente.IdCliente, _appConfig.WebAPI.BaseAddress);
            if (!string.IsNullOrWhiteSpace(dataEvento) && DateTime.TryParseExact(dataEvento, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.AssumeLocal, out dataParsed))
            {
                vm.Data = dataParsed;
            }
            if (!string.IsNullOrWhiteSpace(oraEvento) && TimeSpan.TryParseExact(oraEvento, "c", CultureInfo.InvariantCulture, out timeParsed))
            {
                vm.OraInizio = timeParsed;
            }
            if(!string.IsNullOrWhiteSpace(lid) && int.TryParse(lid, out idLocation))
            {
                vm.IdLocation = idLocation;
            }
            ViewData["TipologieLezioni"] = new SelectList(tipoLezioni, "Id", "Nome");
            ViewData["Locations"] = new SelectList(locations, "Id", "Nome");
            ViewData["IdCliente"] = cliente.IdCliente;
            return View("EditEvento", vm);
        }

        [HttpPost("{cliente}/eventi/new")]
        [HttpPost("{cliente}/eventi/edit/{id}")]
        public async Task<IActionResult> SaveEvento([FromRoute(Name = "cliente")]string urlRoute, [FromForm] EventoViewModel evento, [FromRoute(Name="id")] int? idEvento)
        {
            var cliente = await WebAPIClient.GetClienteAsync(urlRoute, _appConfig.WebAPI.BaseAddress);
            var tipoLezioni = await WebAPIClient.GetTipologieLezioniClienteAsync(_appConfig.WebAPI.BaseAddress, cliente.IdCliente);
            var locations = await WebAPIClient.GetLocationsAsync(cliente.IdCliente, _appConfig.WebAPI.BaseAddress);
            if (!ModelState.IsValid)
            {
                ViewData["TipologieLezioni"] = new SelectList(tipoLezioni, "Id", "Nome");
                ViewData["Locations"] = new SelectList(locations, "Id", "Nome");
                ViewData["IdCliente"] = cliente.IdCliente;
                return View("EditEvento", evento);
            }

            ScheduleViewModel apiVM = new ScheduleViewModel()
            {
                CancellabileFinoAl = evento.DataCancellazioneMax.Value.Add(evento.OraCancellazioneMax.Value),
                Data = evento.Data.Value,
                Title = evento.Title,
                IdCliente = cliente.IdCliente,
                IdLocation = evento.IdLocation.Value,
                Istruttore = evento.Istruttore,
                Note = evento.Note,
                OraInizio = evento.OraInizio.Value,
                PostiDisponibili = evento.PostiDisponibili,
                IdTipoLezione = evento.IdTipoLezione.Value,
                Id = evento.Id
            };
            await WebAPIClient.SaveSchedule(cliente.IdCliente, _appConfig.WebAPI.BaseAddress, apiVM);
            return RedirectToAction("Index", "Schedules");
        }


        [HttpGet("{cliente}/eventi/edit/{id}")]
        public async Task<IActionResult> EditEvento([FromRoute(Name = "cliente")]string urlRoute,
                                                    [FromRoute(Name = "id")] int idEvento)
        {
            var cliente = await WebAPIClient.GetClienteAsync(urlRoute, _appConfig.WebAPI.BaseAddress);
            var tipoLezioni = await WebAPIClient.GetTipologieLezioniClienteAsync(_appConfig.WebAPI.BaseAddress, cliente.IdCliente);
            var locations = await WebAPIClient.GetLocationsAsync(cliente.IdCliente, _appConfig.WebAPI.BaseAddress);
            var evento = await WebAPIClient.GetScheduleAsync(_appConfig.WebAPI.BaseAddress, cliente.IdCliente, idEvento);
            ViewData["TipologieLezioni"] = new SelectList(tipoLezioni, "Id", "Nome");
            ViewData["Locations"] = new SelectList(locations, "Id", "Nome");
            ViewData["IdCliente"] = cliente.IdCliente;
            return View("EditEvento", internalBuildViewModel(evento));
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
