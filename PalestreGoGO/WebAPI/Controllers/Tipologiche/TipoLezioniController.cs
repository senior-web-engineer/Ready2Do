using AutoMapper;
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
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/tipologiche/tipolezioni")]
    //[Authorize]
    public class TipoLezioniController : Controller
    {
        private readonly ILogger<TipoLezioniController> _logger;
        private readonly ITipologieLezioniRepository _repository;

        public TipoLezioniController(ITipologieLezioniRepository repository, ILogger<TipoLezioniController> logger)
        {
            this._logger = logger;
            this._repository = repository;
        }

        [HttpGet()]
        public IActionResult GetAll([FromQuery]int idCliente)
        {
            bool authorized = HttpContext.User.CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            var tipoLezioni = _repository.GetAll(idCliente);
            var result = Mapper.Map<IEnumerable<TipologieLezioni>,IEnumerable<TipologieLezioniViewModel>>(tipoLezioni);
            return new OkObjectResult(result);
        }

        [HttpGet("{id}")]
        public IActionResult GetOne([FromQuery]int idCliente, [FromQuery]int id)
        {
            bool authorized = HttpContext.User.CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            var tipoLezione = _repository.GetSingle(idCliente, id);
            var result = Mapper.Map<TipologieLezioni, TipologieLezioniViewModel>(tipoLezione);
            return new OkObjectResult(result);
        }


        [HttpPost()]
        public IActionResult Create([FromQuery]int idCliente, [FromBody] TipologieLezioniViewModel model)
        {
            bool authorized = HttpContext.User.CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var m = Mapper.Map<TipologieLezioniViewModel, TipologieLezioni>(model);
            _repository.Add(idCliente, m);
            _repository.Commit();
            return Ok();
        }

        [HttpPut()]
        public IActionResult Modify([FromQuery]int idCliente, [FromBody] TipologieLezioniViewModel model)
        {
            bool authorized = HttpContext.User.CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            var m = Mapper.Map<TipologieLezioniViewModel, TipologieLezioni>(model);
            _repository.Update(idCliente, m);
            _repository.Commit();
            return Ok();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromQuery]int idCliente, [FromQuery] int id)
        {
            bool authorized = HttpContext.User.CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return Forbid();
            }
            if (!ModelState.IsValid)
            {
                return BadRequest();
            }
            _repository.Delete(idCliente, id);
            _repository.Commit();
            return new NoContentResult();
        }
    }
}