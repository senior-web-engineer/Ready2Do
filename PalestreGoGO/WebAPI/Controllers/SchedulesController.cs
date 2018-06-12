using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
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
        public async Task<IActionResult> AddSchedule([FromRoute]int idCliente, [FromBody] ScheduleViewModel model)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            var entity = Mapper.Map<ScheduleViewModel, Schedules>(model);
            await _repository.AddScheduleAsync(idCliente, entity);
            return Ok(entity.Id);
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
            var result = Mapper.Map<Schedules, ScheduleViewModel>(schedule);
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
        public IActionResult GetSchedules([FromRoute]int idCliente, [FromQuery(Name = "sd")] string start, [FromQuery(Name ="ed")]string end, [FromQuery(Name ="lid")]int? idLocation)
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
                schedule = _repository.GetSchedules(idCliente, startDate, endDate,idLocation.Value);
            }
            else
            {
                schedule = _repository.GetSchedules(idCliente, startDate, endDate);
            }
            var result = Mapper.Map<IEnumerable<Schedules>, IEnumerable<ScheduleDetailsViewModel>>(schedule);
            return Ok(result);
        }

        [HttpDelete()]
        public async Task<IActionResult> DeleteSchedule([FromRoute] int idCliente, [FromQuery] int idSchedule)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            await _repository.RemoveScheduleAsync(idCliente, idSchedule);
            return Ok();
        }

        [HttpPut()]
        public async Task<IActionResult> UpdateSchedule([FromRoute] int idCliente, [FromBody] ScheduleViewModel schedule)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            var entity = Mapper.Map<ScheduleViewModel, Schedules>(schedule);
            await _repository.UpdateSchedule(idCliente, entity);
            return Ok();
        }
    }
}