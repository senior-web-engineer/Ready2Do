using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti/tipologie")]
    [Authorize]
    public class TipologieClientiAPIController : APIControllerBase
    {
        private readonly ILogger<TipologieClientiAPIController> _logger;
        private readonly ITipologieClientiRepository _repository;

        public TipologieClientiAPIController(ITipologieClientiRepository repository, ILogger<TipologieClientiAPIController> logger)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [HttpGet()]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<TipologiaClienteDM>>> GetAll()
        {
            var result = await  _repository.GetAllAsync();
            return Ok(result);
        }

        //[HttpGet("{id:int}")]
        //public async Task<IActionResult> GetOne([FromRoute]int id)
        //{
        //    var tipoCliente = await _repository.GetOneAsync(id);
        //    var result = Mapper.Map<TipologiaCliente, TipologiaClienteViewModel>(tipoCliente);
        //    return new OkObjectResult(result);
        //}
   }
}