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
            return View("EditEvento", vm);
        }

        [HttpPost("{cliente}/eventi/new")]
        public async Task<IActionResult> NewEvento([FromRoute(Name = "cliente")]string urlRoute, [FromForm] EventoViewModel evento)
        {
            if (!ModelState.IsValid)
            {
                return View("EditEvento", evento);
            }
            var cliente = await WebAPIClient.GetClienteAsync(urlRoute, _appConfig.WebAPI.BaseAddress);
            //TODO: Salvare l'evento
            ScheduleViewModel vm = new ScheduleViewModel()
            {
                CancellabileFinoAl = evento.CancellabileFinoAl.Value,
                Data = evento.Data.Value,
                IdCliente = cliente.IdCliente,
                IdLocation = -1,
                Istruttore = evento.Istruttore,
                Note = evento.Note,
                OraInizio = evento.OraInizio.Value,
                PostiDisponibili = evento.PostiDisponibili,
                IdTipoLezione = evento.IdTipoLezione.Value,
                Id = evento.Id
            };

            return Ok();
        }

    }
}
