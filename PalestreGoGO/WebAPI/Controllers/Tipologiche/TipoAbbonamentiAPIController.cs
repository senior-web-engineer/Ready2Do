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
    [Route("api/clienti/{idCliente}/tipologiche/tipoabbonamenti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TipoAbbonamentiAPIController : APIControllerBase
    {
        private readonly ILogger<TipoAbbonamentiAPIController> _logger;
        private readonly ITipologieAbbonamentiRepository _repository;

        public TipoAbbonamentiAPIController(ITipologieAbbonamentiRepository repository, ILogger<TipoAbbonamentiAPIController> logger)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [HttpGet()]
        public async Task<ActionResult<IEnumerable<TipologiaAbbonamentoDM>>> GetList([FromRoute]int idCliente, int pageSize = 25, int pageNumber = 1,  string sortColumn = "DataCreazione", 
                                                  bool sortAscending = false, bool includiCancellati = false, bool includiNonAttivi = false)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized){return Forbid();}
            var tipoAbbonamenti = await _repository.GetListAsync(idCliente, pageNumber: pageNumber, pageSize: pageSize, includiCancellati: includiCancellati,
                                                                 includiNonAttivi: includiNonAttivi, sortColumn: sortColumn, sortAscending: sortAscending);
            return Ok(tipoAbbonamenti);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<TipologiaAbbonamentoDM>> GetOneAsync([FromRoute]int idCliente, [FromRoute]int id)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized) { return Forbid(); }
            var tipoAbbonamento = await _repository.GetOneAsync(idCliente, id);
            if((tipoAbbonamento == null) || (tipoAbbonamento.IdCliente != idCliente)){return BadRequest(); }
            return Ok(tipoAbbonamento);
        }


        [HttpPost()]
        public async Task<IActionResult> CreateAsync([FromRoute]int idCliente, [FromBody] TipologiaAbbonamentoInputDM model)
        {
            
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized){return Forbid();}
            if (!ModelState.IsValid){return BadRequest(); }
            int id = await _repository.AddAsync(idCliente, model);
            return CreatedAtAction("GetOneAsync", new { idCliente, id }, null);
        }

        [HttpPut("{id:int}")]
        public async Task<IActionResult> ModifyAsync([FromRoute]int idCliente, [FromRoute]int id, [FromBody] TipologiaAbbonamentoInputDM model)
        {
            //TODO: Verificare cosa succede se si modifica un tipo abbonamento per cui ci sono già abbonamenti attivi           
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized){return Forbid();}
            if (!ModelState.IsValid){return BadRequest();}
            if(model.Id != id) { return BadRequest(); }
            await _repository.UpdateAsync(idCliente, model);
            return NoContent();
        }

        [HttpDelete("{id:int}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]int idCliente, [FromRoute] int id)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized){return Forbid();}
            await _repository.DeleteAsync(idCliente, id);
            return NoContent();
        }
    }
}