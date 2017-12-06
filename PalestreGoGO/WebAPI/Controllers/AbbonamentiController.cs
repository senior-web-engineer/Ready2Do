using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.DataAccess.Interfaces;
using Microsoft.Extensions.Logging;
using AutoMapper;
using PalestreGoGo.DataModel;
using Microsoft.AspNetCore.Authorization;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/abbonamenti")]
    [Authorize]
    public class AbbonamentiController : PalestreControllerBase
    {
        private readonly IAbbonamentiRepository _repository;
        private readonly ILogger<AbbonamentiController> _logger;

        public AbbonamentiController(IAbbonamentiRepository repository, ILogger<AbbonamentiController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpPost]
        public async Task<IActionResult> AddAbbonamento([FromRoute]int idCliente, [FromBody] AbbonamentoViewModel abbonamento)
        {

            var entity = Mapper.Map<AbbonamentoViewModel, AbbonamentiUtenti>(abbonamento);
            int id = await _repository.AddAbbonamentoAsync(idCliente, entity);
            return CreatedAtAction("GetAbbonamento", id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAbbonamento([FromRoute]int idCliente, [FromRoute] int id)
        {
            var entity = await _repository.GetAbbonamentoAsync(idCliente, id);
            var result = Mapper.Map<AbbonamentiUtenti, AbbonamentoViewModel>(entity);
            return Ok(result);
        }

        [HttpGet()]
        public IActionResult GetAbbonamentiForUser([FromRoute]int idCliente, [FromQuery]Guid idUtente)
        {
            var entity = _repository.GetAbbonamentiForUser(idCliente, idUtente);
            var result = Mapper.Map<IEnumerable<AbbonamentiUtenti>, IEnumerable<AbbonamentoViewModel>>(entity);
            return Ok(result);
        }


        [HttpPut()]
        public async Task<IActionResult> UpdateAbbonamento([FromRoute]int idCliente, [FromBody] AbbonamentoViewModel abbonamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var entity = Mapper.Map<AbbonamentoViewModel, AbbonamentiUtenti>(abbonamento);
            await _repository.UpdateAbbonamentoAsync(idCliente, entity);
            return Ok();
        }
    }
}