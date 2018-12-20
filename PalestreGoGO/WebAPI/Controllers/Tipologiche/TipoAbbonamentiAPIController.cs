using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPIModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;
using ready2do.model.common;
using System;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/{idCliente}/tipologiche/tipoabbonamenti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TipoAbbonamentiAPIController : PalestreControllerBase
    {
        private readonly ILogger<TipoAbbonamentiAPIController> _logger;
        private readonly ITipologieAbbonamentiRepository _repository;

        public TipoAbbonamentiAPIController(ITipologieAbbonamentiRepository repository, ILogger<TipoAbbonamentiAPIController> logger)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [HttpGet()]
        public async Task<IActionResult> GetList([FromRoute]int idCliente, int pageSize = 25, int pageNumber = 1,  string sortColumn = "DataCreazione", 
                                                  bool sortAscending = false, bool includiCancellati = false, bool includiNonAttivi = false)
        {
            

            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return Forbid();
            }
            var tipoAbbonamenti = await _repository.GetListAsync(idCliente, pageNumber: pageNumber, pageSize: pageSize, includiCancellati: includiCancellati,
                                                                 includiNonAttivi: includiNonAttivi, sortColumn: sortColumn, sortAscending: sortAscending);
            return new OkObjectResult(tipoAbbonamenti);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetOne([FromRoute]int idCliente, [FromRoute]int id)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            var tipoAbbonamento = await _repository.GetOneAsync(idCliente, id);
            if((tipoAbbonamento == null) || (tipoAbbonamento.IdCliente != idCliente))
            {
                return BadRequest();
            }
            //var result = Mapper.Map<TipologieAbbonamenti, TipologieAbbonamentiViewModel>(tipoAbbonamento);
            return new OkObjectResult(tipoAbbonamento);
        }


        [HttpPost()]
        public async Task<IActionResult> Create([FromRoute]int idCliente, [FromBody] TipologieAbbonamentiViewModel model)
        {
            
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var m = Mapper.Map<TipologieAbbonamentiViewModel, TipologieAbbonamenti>(model);
            m.IdCliente = idCliente;
            await _repository.AddAsync(idCliente, m);
            return Ok(m.Id);
        }

        [HttpPut()]
        public IActionResult Modify([FromRoute]int idCliente, [FromBody] TipologieAbbonamentiViewModel model)
        {
            //TODO: Verificare cosa succede se si modifica un tipo abbonamento per cui ci sono già abbonamenti attivi           
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var m = Mapper.Map<TipologieAbbonamentiViewModel, TipologieAbbonamenti>(model);
            var oldEntity = _repository.GetSingle(idCliente, m.Id);
            if(oldEntity == null)
            {
                return BadRequest();
            }
            oldEntity.Costo = model.Costo;
            oldEntity.DurataMesi = model.DurataMesi;
            oldEntity.MaxLivCorsi = model.MaxLivCorsi;
            oldEntity.Nome = model.Nome;
            oldEntity.NumIngressi = model.NumIngressi;
            _repository.Update(idCliente, oldEntity);            
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int idCliente, [FromRoute] int id)
        {
            //TODO: Assicurarsi di cancellare solo se non ci sono abbonamenti attivi in essere
            //TODO: In futuro gestire la cancellazione logica
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _repository.Delete(idCliente, id);
            return new NoContentResult();
        }
    }
}