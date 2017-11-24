//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Threading.Tasks;
//using Microsoft.AspNetCore.Http;
//using Microsoft.AspNetCore.Mvc;
//using Microsoft.AspNetCore.Authorization;
//using Microsoft.Extensions.Logging;
//using PalestreGoGo.WebAPI.Utils;
//using System.Net;

//namespace PalestreGoGo.WebAPI.Controllers
//{
//    [Produces("application/json")]
//    [Route("api/tipologiche")]
//    //[Authorize]
//    public class TipoLezioniController : ControllerBase
//    {
//        private readonly ILogger<TipoLezioniController> _logger;
//        private readonly ITipologieLezioniRepository _repository;

//        public TipoLezioniController(PalestreGoGoDbContext dbContext, ILogger<TipoLezioniController> logger)
//        {
//            this._logger = logger;
//            this._dbContext = dbContext;
//        }

//        #region TIPOLOGIE LEZIONI
//        [HttpGet("tipoLezioni")]
//        public IActionResult GetTipologieLezioni([FromQuery]int idCliente)
//        {
//            bool authorized = HttpContext.User.CanEditTipologiche(idCliente);
//            if (!authorized)
//            {
//                return new StatusCodeResult((int)HttpStatusCode.Forbidden);
//            }
//            var tipoLezioni = _dbContext.TipologieLezioni.Where(tl => tl.IdCliente == idCliente);
//            if((tipoLezioni == null) || (tipoLezioni.Count() == 0))
//            {
//                return new NoContentResult();
//            }
//            var result = from t in tipoLezioni
//                         select new TipologieLezioniViewModel()
//                         {
//                             Id = t.Id,
//                             Nome = t.Nome,
//                             Descrizione = t.Descrizione,
//                             Durata = t.Durata,
//                             MaxPartecipanti = t.MaxPartecipanti,
//                             LimiteCancellazioneMinuti = t.LimiteCancellazioneMinuti,
//                             Livello = t.Livello
//                         };
//            return new OkObjectResult(result);
//        }

//        [HttpPost("tipoLezioni")]
//        public async Task<IActionResult> AddTipologiaLezione([FromQuery]int idCliente, [FromBody] TipologieLezioniViewModel model)
//        {
//            bool authorized = HttpContext.User.CanEditTipologiche(idCliente);
//            if (!authorized)
//            {
//                return Forbid();
//            }
//            if (!ModelState.IsValid)
//            {
//                return BadRequest();
//            }
//            await _dbContext.AddAsync(model.FromViewModel());
//            await _dbContext.SaveChangesAsync();
//            return Created()
//        }
//        #endregion
//    }
//}