using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/schedulers")]
    [Authorize]
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
        public async Task<IActionResult> AddSchedule([FromRoute]int idCliente, ScheduleViewModel model)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            var entity = Mapper.Map<ScheduleViewModel, Schedules>(model);
            await _repository.AddScheduleAsync(idCliente, entity);
            return CreatedAtAction("GetSchedule", entity.Id);
        }

        /// <summary>
        /// Ritorna i dettagli di uno Schedule
        /// </summary>
        /// <remarks>
        /// 
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
        /// Ritorna i dettagli di uno Schedule
        /// </summary>
        /// <remarks>
        /// 
        /// </remarks>
        /// <param name="idCliente"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet()]
        [AllowAnonymous]
        public IActionResult GetSchedules([FromRoute]int idCliente, [FromQuery(Name = "sd")] DateTime startDate, [FromQuery(Name ="ed")]DateTime endDate )
        {
            var schedule = _repository.GetSchedules(idCliente, startDate, endDate);
            var result = Mapper.Map<IEnumerable<Schedules>, IEnumerable<ScheduleViewModel>>(schedule);
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