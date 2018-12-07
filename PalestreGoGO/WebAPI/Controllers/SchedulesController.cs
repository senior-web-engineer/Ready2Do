using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
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
    public class SchedulesController : PalestreControllerBase
    {
        private readonly ILogger<SchedulesController> _logger;
        private readonly ISchedulesRepository _repository;

        public SchedulesController(ILogger<SchedulesController> logger, ISchedulesRepository repository)
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

        [HttpPut("{id}")]
        [HttpPut("{id}/subsequent")]
        public async Task<IActionResult> UpdateSchedule([FromRoute] int idCliente, [FromBody] ScheduleDM schedule)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            if(ControllerContext.RouteData
            var entity = Mapper.Map<ScheduleApiModel, Schedules>(schedule);
            await _repository.UpdateSchedule(idCliente, entity);
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
            var result = Mapper.Map<Schedules, ScheduleApiModel>(schedule);
            return Ok(result);
        }

        /// <summary>
        /// Ritorna gli Schedules per un range temporale
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="idCliente"></param>
        /// <param name="id"></param>
        /// <param name="startDate">stringa in formto "yyyyMMddTHHmmss"</param>
        /// <param name="endDate">stringa in formto "yyyyMMddTHHmmss"</param>
        /// <returns></returns>
        [HttpGet()]
        [AllowAnonymous]
        public async Task<IActionResult> GetSchedules([FromRoute]int idCliente, [FromQuery(Name = "sd")] string start, [FromQuery(Name ="ed")]string end, [FromQuery(Name ="lid")]int? idLocation)
        {
            DateTime startDate, endDate;
            IEnumerable<Schedules> schedule;
            if (!DateTime.TryParseExact(start,Constants.DATETIME_QUERYSTRING_FORMAT,CultureInfo.InvariantCulture, DateTimeStyles.None,out startDate))
            {
                return BadRequest();
            }
            if (!DateTime.TryParseExact(end, Constants.DATETIME_QUERYSTRING_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                return BadRequest();
            }
            /*Limitiamo il range temporale per cui ritornare gli schedules a 60 giorni*/
            if(endDate.Subtract(startDate).TotalDays > 60)
            {
                return BadRequest();
            }

            if (idLocation.HasValue)
            {
                schedule = await _repository.GetSchedulesAsync(idCliente, startDate, endDate,idLocation.Value);
            }
            else
            {
                schedule = await _repository.GetSchedulesAsync(idCliente, startDate, endDate);
            }
            var result = Mapper.Map<IEnumerable<Schedules>, IEnumerable<ScheduleDetailedApiModel>>(schedule);
            return Ok(result);
        }


        /// <summary>
        /// Ritorna SOLO gli schedule "pubblicati" ossia visibili pubblicamente ovvero quelli per cui sono aperte le iscrizioni
        /// o per cui risulta VisibileDal < NOW
        /// </summary>
        /// <param name="idCliente"></param>
        /// <param name="start"></param>
        /// <param name="end"></param>
        /// <param name="idLocation"></param>
        /// <returns></returns>
        [HttpGet("published")]
        public async Task<IActionResult> GetPublicSchedules([FromRoute]int idCliente, [FromQuery(Name = "sd")] string start, [FromQuery(Name = "ed")]string end, [FromQuery(Name = "lid")]int? idLocation)
        {
            DateTime startDate, endDate;
            IEnumerable<Schedules> schedule;
            if (!DateTime.TryParseExact(start, Constants.DATETIME_QUERYSTRING_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out startDate))
            {
                return BadRequest();
            }
            if (!DateTime.TryParseExact(end, Constants.DATETIME_QUERYSTRING_FORMAT, CultureInfo.InvariantCulture, DateTimeStyles.None, out endDate))
            {
                return BadRequest();
            }
            /*Limitiamo il range temporale per cui ritornare gli schedules a 60 giorni*/
            if (endDate.Subtract(startDate).TotalDays > 60)
            {
                return BadRequest();
            }

            if (idLocation.HasValue)
            {
                schedule = await _repository.GetSchedulesAsync(idCliente, startDate, endDate, idLocation.Value);
            }
            else
            {
                schedule = await _repository.GetSchedulesAsync(idCliente, startDate, endDate);
            }
            var result = Mapper.Map<IEnumerable<Schedules>, IEnumerable<ScheduleDetailedApiModel>>(schedule);
            return Ok(result);
        }


        [HttpDelete()]
        public async Task<IActionResult> DeleteSchedule([FromRoute] int idCliente, [FromQuery] int idSchedule)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            await _repository.RemoveScheduleAsync(idCliente, idSchedule);
            return Ok();
        }

        
    }
}