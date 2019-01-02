using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.WebAPI.Utils;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti{idCliente:int}/images")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ImmaginiClientiAPIController : APIControllerBase
    {
        private readonly IImmaginiClientiRepository _immaginiRepository;
        private readonly ILogger<ImmaginiClientiAPIController> _logger;

        public ImmaginiClientiAPIController(ILogger<ImmaginiClientiAPIController> logger,
                                            IImmaginiClientiRepository immaginiRepository)
        {
            _logger = logger;
            _immaginiRepository = immaginiRepository;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<ActionResult<IEnumerable<ImmagineClienteDM>>> GetImagesCliente([FromRoute(Name = "idCliente")] int idCliente, [FromQuery(Name ="tipo")]TipoImmagineDM? tipo = null)
        {
            var result = await _immaginiRepository.GetImages(idCliente, tipo);
            return Ok(result);
        }

        [HttpGet("sfondo")]
        [AllowAnonymous]
        public async Task<ActionResult<ImmagineClienteDM>> GetImageSfondoCliente([FromRoute(Name = "idCliente")] int idCliente)
        {
            var result = await _immaginiRepository.GetImages(idCliente, TipoImmagineDM.Sfondo);
            return Ok(result.SingleOrDefault());
        }

        [HttpGet("{idImage:int}")]
        [AllowAnonymous]
        public async Task<ActionResult<ImmagineClienteDM>> GetImageCliente([FromRoute(Name = "idCliente")] int idCliente, [FromRoute(Name = "idImage")]int idImage)
        {
            var result = await _immaginiRepository.GetImage(idCliente, idImage);
            return Ok(result);
        }

        [HttpPost]
        public async Task<IActionResult> AddImageCliente([FromRoute(Name = "idCliente")] int idCliente, ImmagineClienteInputDM immagine)
        {
            var idImage = await _immaginiRepository.AddImageAsync(idCliente, immagine);
            return CreatedAtAction("GetImageCliente", new { idImage = idImage }, immagine);
        }

        [HttpPut("{idImage:int}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(400)]
        [ProducesResponseType(403)]
        public async Task<IActionResult> UpdateImageCliente([FromRoute(Name = "idCliente")] int idCliente, [FromRoute(Name = "idImage")]int idImage, [FromBody] ImmagineClienteInputDM immagine)
        {
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            if (!ModelState.IsValid) { return BadRequest(); }
            if((immagine == null) || (!immagine.Id.HasValue)) { return BadRequest(); }
            await _immaginiRepository.UpdateImageAsync(idCliente, immagine);
            return NoContent();
        }

        [HttpDelete("{idImage:int}")]
        [ProducesResponseType(202)]
        [ProducesResponseType(403)]
        public async Task<ActionResult<ImmagineClienteDM>> DeleteImageCliente([FromRoute(Name = "idCliente")] int idCliente, [FromRoute(Name="idImage")]int idImage)
        {
            if (!User.CanManageStructure(idCliente)) { return Forbid(); }
            ImmagineClienteDM deleted = await _immaginiRepository.DeleteImageAsync(idCliente, idImage);
            return Ok(deleted);
        }
    }
}
