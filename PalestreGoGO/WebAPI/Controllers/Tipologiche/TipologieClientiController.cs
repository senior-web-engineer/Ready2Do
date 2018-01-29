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
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti")]
    [Authorize]
    public class TipologieClientiController : Controller
    {
        private readonly ILogger<TipologieClientiController> _logger;
        private readonly ITipologieClientiRepository _repository;

        public TipologieClientiController(ITipologieClientiRepository repository, ILogger<TipologieClientiController> logger)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [HttpGet("tipologie")]
        [AllowAnonymous]
        public IActionResult GetAll()
        {
            var tipiClienti = _repository.GetAll();
            var result = Mapper.Map<IEnumerable<TipologiaCliente>,IEnumerable<TipologiaClienteViewModel>>(tipiClienti);
            return new OkObjectResult(result);
        }

        [HttpGet("tipologie/{id}")]
        public async Task<IActionResult> GetOne([FromQuery]int id)
        {
            var tipoCliente = await _repository.GetOneAsync(id);
            var result = Mapper.Map<TipologiaCliente, TipologiaClienteViewModel>(tipoCliente);
            return new OkObjectResult(result);
        }
   }
}