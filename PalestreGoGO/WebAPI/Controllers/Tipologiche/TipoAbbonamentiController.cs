﻿using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPI.ViewModel;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/{idCliente}/tipologiche/tipoabbonamenti")]
    [Authorize]
    public class TipoAbbonamentiController : Controller
    {
        private readonly ILogger<TipoAbbonamentiController> _logger;
        private readonly ITipologieAbbonamentiRepository _repository;

        public TipoAbbonamentiController(ITipologieAbbonamentiRepository repository, ILogger<TipoAbbonamentiController> logger)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [HttpGet()]
        public IActionResult GetAll([FromRoute]int idCliente)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            var tipoAbbonamenti = _repository.GetAll(idCliente);
            var result = Mapper.Map<IEnumerable<TipologieAbbonamenti>,IEnumerable<TipologieAbbonamentiViewModel>>(tipoAbbonamenti);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromRoute]int idCliente, [FromQuery]int id)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            var tipoAbbonamento = _repository.GetSingle(idCliente, id);
            if((tipoAbbonamento == null) || (tipoAbbonamento.IdCliente != idCliente))
            {
                return BadRequest();
            }
            var result = Mapper.Map<TipologieAbbonamenti, TipologieAbbonamentiViewModel>(tipoAbbonamento);
            return new OkObjectResult(result);
        }


        [HttpPost()]
        public IActionResult Create([FromRoute]int idCliente, [FromBody] TipologieAbbonamentiViewModel model)
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
            _repository.Add(idCliente, m);
            return CreatedAtAction("GetOne", m.Id);
        }

        [HttpPut()]
        public IActionResult Modify([FromRoute]int idCliente, [FromBody] TipologieAbbonamentiViewModel model)
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
        public IActionResult Delete([FromRoute]int idCliente, [FromQuery] int id)
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
            _repository.Delete(idCliente, id);
            return new NoContentResult();
        }

        public virtual ClaimsPrincipal GetCurrentUser()
        {
            return HttpContext.User;
        }
    }
}