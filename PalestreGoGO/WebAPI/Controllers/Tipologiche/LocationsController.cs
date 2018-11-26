using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
using System.Collections.Generic;
using System.Net;
using System.Security.Claims;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/{idCliente:int}/tipologiche/locations")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class LocationsController: PalestreControllerBase
    {

        private readonly ILogger<LocationsController> _logger;
        private readonly ILocationsRepository _repository;

        public LocationsController(ILocationsRepository repository, ILogger<LocationsController> logger)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [HttpGet()]
        [AllowAnonymous]
        public IActionResult GetAll([FromRoute]int idCliente)
        {
            //bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            //if (!authorized)
            //{
            //    return Forbid();
            //}
            var locations = _repository.GetAll(idCliente);
            var result = Mapper.Map<IEnumerable<Locations>, IEnumerable<LocationApiModel>>(locations);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        public IActionResult GetOne([FromRoute]int idCliente, [FromRoute]int id)
        {
            //bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            //if (!authorized)
            //{
            //    return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            //}
            var location = _repository.GetSingle(idCliente, id);
            if ((location == null) || (location.IdCliente != idCliente))
            {
                return BadRequest();
            }
            var result = Mapper.Map<Locations, LocationApiModel>(location);
            return new OkObjectResult(result);
        }


        [HttpPost()]
        public IActionResult Create([FromRoute]int idCliente, [FromBody] LocationApiModel model)
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
            var m = Mapper.Map<LocationApiModel, Locations>(model);
            m.IdCliente = idCliente;
            _repository.Add(idCliente, m);
            //return CreatedAtAction("GetOne", m.Id);
            return Ok();
        }

        [HttpPut()]
        public IActionResult Modify([FromRoute]int idCliente, [FromBody] LocationApiModel model)
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
            var m = Mapper.Map<LocationApiModel, Locations>(model);
            var oldEntity = _repository.GetSingle(idCliente, m.Id);
            if (oldEntity == null)
            {
                return BadRequest();
            }
            oldEntity.CapienzaMax = model.CapienzaMax;
            oldEntity.Nome = model.Nome;
            oldEntity.Descrizione = model.Descrizione;
            _repository.Update(idCliente, oldEntity);
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute]int idCliente, [FromRoute] int id)
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
    }
}
