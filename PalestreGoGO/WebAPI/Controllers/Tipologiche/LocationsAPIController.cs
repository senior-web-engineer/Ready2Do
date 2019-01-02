using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Utils;
using ready2do.model.common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/{idCliente:int}/tipologiche/locations")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LocationsAPIController : APIControllerBase
    {

        private readonly ILogger<LocationsAPIController> _logger;
        private readonly ILocationsRepository _repository;

        public LocationsAPIController(ILocationsRepository repository, ILogger<LocationsAPIController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet()]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<LocationDM>>> GetAllAsync([FromRoute]int idCliente)
        {
            var locations = await _repository.GetAllAsync(idCliente);
            return Ok(locations);
        }

        [HttpGet("{id:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<LocationDM>> GetOneAsync([FromRoute]int idCliente, [FromRoute]int id)
        {
            var location = await _repository.GetSingleAsync(idCliente, id);
            if ((location == null) || (location.IdCliente != idCliente))
            {
                return BadRequest();
            }
            return Ok(location);
        }


        [HttpPost()]
        public async Task<IActionResult> CreateAsync([FromRoute]int idCliente, [FromBody] LocationInputDM model)
        {

            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized) { return Forbid(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            if (model.IdCliente != idCliente) { return BadRequest(); }
            int id = await _repository.AddAsync(idCliente, model);
            return CreatedAtAction("GetOneAsync", new { idCliente, id }, model);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ModifyAsync([FromRoute]int idCliente, [FromBody] LocationInputDM model)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized) { return Forbid(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            await _repository.UpdateAsync(idCliente, model);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]int idCliente, [FromRoute] int id)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized) { return Forbid(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            await _repository.DeleteAsync(idCliente, id);
            return NoContent();
        }
    }
}
