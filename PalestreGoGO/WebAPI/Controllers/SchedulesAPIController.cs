using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
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
        /// Ritorna uno Schedule
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
                                                        [FromQuery(Name = "onlyavailable")]bool? soloPostiDisp = false, [FromQuery(Name = "onlyopen")]bool? soloIscrizAperte = true,
                                                        [FromQuery(Name = "psize")]int? pageSize = 25, [FromQuery(Name = "pnum")]int? pageNumber = 1,
                                                        [FromQuery(Name = "sortcol")]string sortColumn = "dataorainizioschedules", [FromQuery(Name = "asc")]bool? ascending = true,
                                                        [FromQuery(Name = "deleted")]bool? includeDeleted = false)
        {
            //Gli item cancellati sono visibili solo dal gestore della struttura
            if (!User.CanManageStructure(idCliente) && includeDeleted.Value) return BadRequest();
            //Gli utenti NON amministratori possono vedere solo gli schedule per cui sono aperte le iscrizioni
            if (!User.CanManageStructure(idCliente) && !(soloIscrizAperte.Value)) return BadRequest();

            DateTime startDate, endDate;
            IEnumerable<ScheduleDM> schedules;
            if (string.IsNullOrEmpty(start))
            {
                startDate = DateTime.Now.AddDays(-10); // Default: Oggi -10 gg
                _logger.LogTrace($"Empty StartDate. Used default [{startDate}]");
            }
            else if (!DateTime.TryParse(start, null, DateTimeStyles.RoundtripKind , out startDate))
            {
                _logger.LogWarning($"Invalid StartDate parameter[{startDate}]. Return BAD_REQUEST");
                return BadRequest();
            }
            if (string.IsNullOrEmpty(end))
            {
                endDate = DateTime.Now.AddDays(10); // Default: Oggi +10 gg
                _logger.LogTrace($"Empty EndDate. Used default [{endDate}]");
            }
            else if (!DateTime.TryParse(end, null, DateTimeStyles.RoundtripKind, out endDate))
            {
                _logger.LogWarning($"Invalid EndDate parameter[{end}]. Return BAD_REQUEST");
                return BadRequest();
            }

            /*Limitiamo il range temporale per cui ritornare gli schedules a 60 giorni*/
            var rangeDays = endDate.Subtract(startDate).TotalDays;
            if (rangeDays > 60)
            {
                _logger.LogWarning($"Richiesto un range troppo ampio [{rangeDays}]. StartDate: {startDate}, EndDate: {endDate}");
                return BadRequest();
            }

            schedules = await _repository.GetScheduleListAsync(idCliente, startDate, endDate, idLocation, idTipoLezione, soloPostiDisp.Value, soloIscrizAperte.Value, pageSize.Value, pageNumber.Value, sortColumn, ascending.Value, includeDeleted.Value);
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