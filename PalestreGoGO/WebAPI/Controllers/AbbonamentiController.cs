using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Controllers;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/abbonamenti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
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
        public async Task<IActionResult> AddAbbonamento([FromRoute]int idCliente, [FromBody] AbbonamentoUtenteApiModel abbonamento)
        {

            var entity = Mapper.Map<AbbonamentoUtenteApiModel, AbbonamentiUtenti>(abbonamento);
            int id = await _repository.AddAbbonamentoAsync(idCliente, entity);
            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAbbonamento([FromRoute]int idCliente, [FromRoute] int id)
        {
            var entity = await _repository.GetAbbonamentoAsync(idCliente, id);
            var result = Mapper.Map<AbbonamentiUtenti, AbbonamentoViewModel>(entity);
            return Ok(result);
        }

        [HttpGet()]
        public IActionResult GetAbbonamentiForUser([FromRoute]int idCliente, [FromQuery]string idUtente)
        {
            var entity = _repository.GetAbbonamentiForUser(idCliente, idUtente);
            var result = Mapper.Map<IEnumerable<AbbonamentiUtenti>, IEnumerable<AbbonamentoViewModel>>(entity);
            return Ok(result);
        }


        [HttpPut()]
        public async Task<IActionResult> UpdateAbbonamento([FromRoute]int idCliente, [FromBody] AbbonamentoUtenteApiModel abbonamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var entity = Mapper.Map<AbbonamentoUtenteApiModel, AbbonamentiUtenti>(abbonamento);
            await _repository.UpdateAbbonamentoAsync(idCliente, entity);
            return Ok();
        }
    }
}