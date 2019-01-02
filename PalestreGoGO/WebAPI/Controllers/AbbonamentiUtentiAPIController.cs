using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.WebAPI.Utils;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/abbonamenti/{userId}")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AbbonamentiUtentiAPIController : APIControllerBase
    {
        private readonly IAbbonamentiRepository _repository;
        private readonly ILogger<AbbonamentiUtentiAPIController> _logger;

        public AbbonamentiUtentiAPIController(IAbbonamentiRepository repository, ILogger<AbbonamentiUtentiAPIController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddAbbonamento([FromRoute]int idCliente, [FromRoute(Name ="userId")]string userId, [FromBody] AbbonamentoUtenteInputDM abbonamento)
        {
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            if (!abbonamento.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase)) { return BadRequest(); }
            int id = await _repository.SaveAbbonamentoAsync(idCliente, abbonamento);
            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<AbbonamentoUtenteDM>> GetAbbonamento([FromRoute]int idCliente, [FromRoute(Name = "userId")]string userId, [FromRoute(Name = "id")] int idAbbonamento)
        {
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            var entity = await _repository.GetAbbonamentoAsync(idCliente, idAbbonamento);
            if (!entity.UserId.Equals(userId, StringComparison.InvariantCultureIgnoreCase)) { return BadRequest(); }
            return Ok(entity);
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<AbbonamentoUtenteDM>>> GetAbbonamentiForUser([FromRoute]int idCliente, [FromRoute(Name = "userId")]string userId,
                                                                [FromQuery(Name = "incExp")]bool includeExpired = false, [FromQuery(Name = "incDel")]bool includeDeleted = false)
        {
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            IEnumerable<AbbonamentoUtenteDM> result = await _repository.GetAbbonamentiForUserAsync(idCliente, userId, includeExpired, includeDeleted);
            return Ok(result);
        }


        [HttpPut()]
        public async Task<IActionResult> UpdateAbbonamento([FromRoute]int idCliente, [FromRoute(Name = "userId")]string userId, [FromBody] AbbonamentoUtenteInputDM abbonamento)
        {
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            await _repository.SaveAbbonamentoAsync(idCliente, abbonamento);
            return Ok();
        }

        [HttpDelete("{userId}/{idAbbonamento}")]
        public async Task<IActionResult> DeleteAbbonamento([FromRoute]int idCliente, [FromRoute]string userId, [FromRoute] int idAbbonamento)
        {
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            await _repository.DeleteAbbonamentoAsync(idCliente, userId, idAbbonamento);
            return Ok();
        }
    }
}