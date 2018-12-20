using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.DataAccess;
using PalestreGoGo.DataModel;
using PalestreGoGo.WebAPI.Utils;
using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/clienti/{idCliente}/users/{userId}/certificati")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CertificatiUtentiAPIController : PalestreControllerBase
    {        
        private readonly IClientiUtentiRepository _repository;
        private readonly ILogger<CertificatiUtentiAPIController> _logger;

        public CertificatiUtentiAPIController(IClientiUtentiRepository repository, ILogger<CertificatiUtentiAPIController> logger)
        {
            _logger = logger;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetCertificati([FromRoute]int idCliente, [FromRoute]string userId,
                                                        [FromQuery(Name ="expired")]bool includeExpired = true, 
                                                        [FromQuery(Name="deleted")]bool includeDeleted = false)
        {
            if (!GetCurrentUser().CanManageStructure(idCliente)) return Forbid();
            var certificati = await _repository.GetCertificatiUtente(idCliente, userId, includeExpired, includeDeleted);
            var result = Mapper.Map<IEnumerable<UtenteClienteCertificatoDM>, IEnumerable<ClienteUtenteApiModel>>(certificati);
            return Ok(result);
        }
    }
}
