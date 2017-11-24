using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Provisioning")]
    [Authorize(Policy ="ProvisioningPolicy")]
    //[Authorize]
    public class ProvisioningController : ControllerBase
    {
        [HttpPost]
        
        public IActionResult ProvisionClient([FromQuery] int idCliente, [FromQuery] Guid owner, [FromQuery] string token)
        {
            //var cliente = _dbContext.Clienti.Single(c => c.Id == idCliente && c.IdUserOwner == owner && c.ProvisioningToken.Equals(token));
            //if (cliente == null) return BadRequest();

            return new OkResult();
        }
    }
}