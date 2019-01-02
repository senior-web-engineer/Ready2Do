using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Utils;
using ready2do.model.common;
using System.Net;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti/{idCliente:int}/tipologiche/tipolezioni")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TipoLezioniAPIController : APIControllerBase
    {
        private readonly ITipologieLezioniRepository _repository;

        public TipoLezioniAPIController(ITipologieLezioniRepository repository)
        {
            _repository = repository;
        }

        [AllowAnonymous]
        [HttpGet()]
        public async Task<IActionResult> GetList([FromRoute]int idCliente, [FromQuery(Name = "page")] int pageNumber = 1,
                                                 [FromQuery(Name = "pageSize")] int pageSize = 100,
                                                 [FromQuery(Name = "sortby")] string sortColumn = null,
                                                 [FromQuery(Name = "sortype")] string sortType = "asc")
        {
            //TODO: Rivedere la gestione della security
            //bool authorized = GetCurrentUser().CanReadTipologiche(idCliente);
            //if (!authorized)
            //{
            //    return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            //}
            var tipoLezioni = await _repository.GetListAsync(idCliente, sortColumn, (sortType ?? "asc").ToLowerInvariant() == "asc", pageNumber, pageSize);
           // var result = Mapper.Map<IEnumerable<TipologieLezioni>, IEnumerable<TipologieLezioniApiModel>>(tipoLezioni);
            return new OkObjectResult(tipoLezioni);
        }

        [AllowAnonymous]
        [HttpGet("{id}", Name = "GetTipoLezione")]
        public async Task<IActionResult> GetOne([FromRoute]int idCliente, [FromRoute]int id)
        {
            bool authorized = GetCurrentUser().CanReadTipologiche(idCliente);
            if (!authorized)
            {
                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
            }
            var tipoLezione = await _repository.GetAsync(idCliente, id);
            if ((tipoLezione == null) || (tipoLezione.IdCliente != idCliente))
            {
                return BadRequest();
            }
           // var result = Mapper.Map<TipologieLezioni, TipologieLezioniApiModel>(tipoLezione);
            return new OkObjectResult(tipoLezione);
        }


        [HttpPost()]
        public async Task<IActionResult> CreateAsync([FromRoute]int idCliente, [FromBody] TipologiaLezioneDM model)
        {
            if (!GetCurrentUser().CanEditTipologiche(idCliente)) return Forbid();
            if (!ModelState.IsValid) return BadRequest();
            if (!model.Id.HasValue || model.Id.Value!= idCliente) return BadRequest();
            //var m = Mapper.Map<TipologieLezioniApiModel, TipologieLezioni>(model);
            //m.IdCliente = idCliente;
            await _repository.AddAsync(idCliente, model);
            //return CreatedAtAction("GetTipoLezione", new { idCliente, id = m.Id });
            return Ok();
        }

        [HttpPut()]
        public async Task<IActionResult> ModifyAsync([FromRoute]int idCliente, [FromBody] TipologiaLezioneDM model)
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
            if (!model.Id.HasValue || model.Id.Value != idCliente) return BadRequest();

            //  var m = Mapper.Map<TipologieLezioniApiModel, TipologieLezioni>(model);

            await _repository.UpdateAsync(idCliente, model);
            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteAsync([FromRoute]int idCliente, [FromQuery] int id)
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
            await _repository.DeleteAsync(idCliente, id);
            return new NoContentResult();
        }

        [HttpGet("checkname/{nome}")]
        public async Task<IActionResult> CheckName([FromRoute]int idCliente, [FromRoute] string nome)
        {
            bool authorized = GetCurrentUser().CanEditTipologiche(idCliente);
            if (!authorized)
            {
                return Forbid();
            }
            if(string.IsNullOrWhiteSpace(nome))  { return BadRequest(); }
            return Ok(await _repository.CheckNameAsync(idCliente, nome));
        }

    }
}