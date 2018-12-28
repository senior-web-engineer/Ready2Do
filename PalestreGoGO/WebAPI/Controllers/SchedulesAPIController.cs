using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Model.Extensions;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti/{idCliente:int}/schedules")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SchedulesAPIController : APIControllerBase
    {
        private readonly ILogger<SchedulesAPIController> _logger;
        private readonly ISchedulesRepository _repository;

        public SchedulesAPIController(ILogger<SchedulesAPIController> logger, ISchedulesRepository repository)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost()]
        public async Task<IActionResult> AddSchedule([FromRoute]int idCliente, [FromBody] ScheduleDM schedule)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            await _repository.AddScheduleAsync(idCliente, schedule);
            return Ok(schedule.Id);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] int idCliente, [FromRoute] int id, [FromBody] ScheduleChangeApiModel scheduleChange)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            if(scheduleChange.Schedule == null) { return BadRequest(); }
            if(id != scheduleChange.Schedule.Id) { return BadRequest(); }
            await _repository.UpdateScheduleAsync(idCliente, scheduleChange.Schedule, scheduleChange.TipoModifica);
            return Ok();
        }

        /// <summary>
        /// Ritorna i dettagli di uno Schedule
        /// </summary>
        /// <remarks>
        /// ATTENZIONE! Al momento ritorna tutti i dettagli dello schedule. 
        /// Per motivi di sicurezza, alcuni dati potrebbero essere resi visibili solo agli utenti autorizzato (owner, backend, ecc...)
        /// In questo caso bisogna limitare il dati ritornati in base ai claims
        /// </remarks>
        /// <param name="idCliente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> GetSchedule([FromRoute]int idCliente, [FromRoute] int id)
        {
            var schedule = await _repository.GetScheduleAsync(idCliente, id);
            if (!GetCurrentUser().CanManageStructure(idCliente) && !schedule.IsPublicVisible())
            {
                return NotFound();
            }
            return Ok(schedule);
        }

        /// <summary>
        /// Ritorna gli Schedules per un range temporale
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="idCliente"></param>
        /// <param name="id"></param>
        /// <param name="startDate">stringa in formato ISO8601 (preferibilmente UTC) "yyyyMMddTHHmmssZ"</param>
        /// <param name="endDate">stringa in formato ISO8601 (preferibilmente UTC) "yyyyMMddTHHmmssZ"</param>
        /// <returns></returns>
        [HttpGet()]
        [AllowAnonymous]
        public async Task<IActionResult> GetSchedules([FromRoute]int idCliente, [FromQuery(Name = "sd")] string start = null, [FromQuery(Name = "ed")]string end = null,
                                                        [FromQuery(Name = "lid")]int? idLocation = null, [FromQuery(Name = "tlid")]int? idTipoLezione = null,
                                                        [FromQuery(Name = "onlyavailable")]bool soloPostiDisp = false, [FromQuery(Name = "onlyopen")]bool soloIscrizAperte = true,
                                                        [FromQuery(Name = "psize")]int pageSize = 25, [FromQuery(Name = "pnum")]int pageNumber = 1,
                                                        [FromQuery(Name = "sortcol")]string sortColumn = "dataorainizio", [FromQuery(Name = "asc")]bool ascending = true,
                                                        [FromQuery(Name = "deleted")]bool includeDeleted = false)
        {
            //Gli item cancellati sono visibili solo dal gestore della struttura
            if (!User.CanManageStructure(idCliente) && includeDeleted) return BadRequest();
            //Gli utenti NON amministratori possono vedere solo gli schedule per cui sono aperte le iscrizioni
            if (!User.CanManageStructure(idCliente) && !soloIscrizAperte) return BadRequest();

            DateTime startDate, endDate;
            IEnumerable<ScheduleDM> schedules;
            if (string.IsNullOrEmpty(end))
            {
                startDate = DateTime.Now.AddDays(-10); // Default: Oggi -10 gg
            }
            else if (!DateTime.TryParseExact(start.Replace("T", " "), "u", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out startDate))
            {
                return BadRequest();
            }
            if (string.IsNullOrEmpty(end))
            {
                endDate = DateTime.Now.AddDays(10); // Default: Oggi +10 gg
            }
            else if (!DateTime.TryParseExact(end.Replace("T", " "), "u", CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind, out endDate))
            {
                return BadRequest();
            }

            /*Limitiamo il range temporale per cui ritornare gli schedules a 60 giorni*/
            if (endDate.Subtract(startDate).TotalDays > 60)
            {
                return BadRequest();
            }

            schedules = await _repository.GetScheduleListAsync(idCliente, startDate, endDate, idLocation, idTipoLezione, soloPostiDisp, soloIscrizAperte, pageSize, pageNumber, sortColumn, ascending, includeDeleted);
            return Ok(schedules);
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteSchedule([FromRoute] int idCliente, [FromQuery] int idSchedule)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            await _repository.DeleteScheduleAsync(idCliente, idSchedule);
            return Ok();
        }


    }
}