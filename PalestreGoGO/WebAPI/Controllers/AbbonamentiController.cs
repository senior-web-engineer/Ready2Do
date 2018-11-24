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

            var entity = Mapper.Map<AbbonamentoUtenteApiModel, UtenteClienteAbbonamentoDM>(abbonamento);
            int id = await _repository.SaveAbbonamentoAsync(idCliente, entity);
            return Ok(id);
        }

        [HttpGet("{id}")]
        public async Task<IActionResult> GetAbbonamento([FromRoute]int idCliente, [FromRoute(Name ="id")] int idAbbonamento)
        {
            var entity = await _repository.GetAbbonamentoAsync(idCliente, idAbbonamento);
            var result = Mapper.Map<UtenteClienteAbbonamentoDM, AbbonamentoViewModel>(entity);
            return Ok(result);
        }

        [HttpGet()]
        public async Task<IActionResult> GetAbbonamentiForUser([FromRoute]int idCliente, [FromQuery]string idUtente, 
                                                                [FromQuery(Name ="incExp")]bool includeExpired = false, [FromQuery(Name = "incDel")]bool includeDeleted = false)
        {
            IEnumerable<UtenteClienteAbbonamentoDM> entity = await _repository.GetAbbonamentiForUserAsync(idCliente, idUtente, includeExpired, includeDeleted);
            var result = Mapper.Map<IEnumerable<UtenteClienteAbbonamentoDM>, IEnumerable<AbbonamentoViewModel>>(entity);
            return Ok(result);
        }


        [HttpPut()]
        public async Task<IActionResult> UpdateAbbonamento([FromRoute]int idCliente, [FromBody] AbbonamentoUtenteApiModel abbonamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var entity = Mapper.Map<AbbonamentoUtenteApiModel, UtenteClienteAbbonamentoDM>(abbonamento);
            await _repository.SaveAbbonamentoAsync(idCliente, entity);
            return Ok();
        }

        [HttpDelete("{userId}/{idAbbonamento}")]
        public async Task<IActionResult> DeleteAbbonamento([FromRoute]int idCliente, [FromRoute]string userId, [FromRoute] int idAbbonamento)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            await _repository.DeleteAbbonamentoAsync(idCliente, userId, idAbbonamento);
            return Ok();
        }
    }
}