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

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/{idCliente:int}/tipologiche/tipolezioni")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TipoLezioniController : PalestreControllerBase
    {
        private readonly ILogger<TipoLezioniController> _logger;
        private readonly ITipologieLezioniRepository _repository;

        public TipoLezioniController(ITipologieLezioniRepository repository, ILogger<TipoLezioniController> logger)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [AllowAnonymous]
        [HttpGet()]
        public IActionResult GetAll([FromRoute]int idCliente)
        {
            //TODO: Rivedere la gestione della security
            //bool authorized = GetCurrentUser().CanReadTipologiche(idCliente);
            //if (!authorized)
            //{
            //    return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            //}
            var tipoLezioni = _repository.GetAll(idCliente);
            var result = Mapper.Map<IEnumerable<TipologieLezioni>, IEnumerable<TipologieLezioniViewModel>>(tipoLezioni);
            return new OkObjectResult(result);
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetTipoLezione")]
        public IActionResult GetOne([FromRoute]int idCliente, [FromRoute]int id)
        {
            bool authorized = GetCurrentUser().CanReadTipologiche(idCliente);
            if (!authorized)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            var tipoLezione = _repository.GetSingle(idCliente, id);
            if ((tipoLezione == null) || (tipoLezione.IdCliente != idCliente))
            {
                return BadRequest();
            }
            var result = Mapper.Map<TipologieLezioni, TipologieLezioniViewModel>(tipoLezione);
            return new OkObjectResult(result);
        }


        [HttpPost()]
        public IActionResult Create([FromRoute]int idCliente, [FromBody] TipologieLezioniViewModel model)
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
            var m = Mapper.Map<TipologieLezioniViewModel, TipologieLezioni>(model);
            m.IdCliente = idCliente;
            _repository.Add(idCliente, m);
            //return CreatedAtAction("GetTipoLezione", new { idCliente, id = m.Id });
            return Ok();
        }

        [HttpPut()]
        public IActionResult Modify([FromRoute]int idCliente, [FromBody] TipologieLezioniViewModel model)
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
            var m = Mapper.Map<TipologieLezioniViewModel, TipologieLezioni>(model);
            var oldEntity = _repository.GetSingle(idCliente, m.Id);
            if (oldEntity == null)
            {
                return BadRequest();
            }
            oldEntity.Descrizione = model.Descrizione;
            oldEntity.Durata = model.Durata;
            oldEntity.Livello = model.Livello;
            oldEntity.MaxPartecipanti = model.MaxPartecipanti;
            oldEntity.Nome = model.Nome;
            oldEntity.LimiteCancellazioneMinuti = model.LimiteCancellazioneMinuti;
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
    }
}