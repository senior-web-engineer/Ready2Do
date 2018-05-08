using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using PalestreGoGo.WebAPI.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/utenti")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UtentiController : PalestreControllerBase
    {
        private readonly ILogger<UtentiController> _logger;
        private readonly IUsersManagementService _userManagementService;

        public UtentiController(ILogger<UtentiController> logger,
                                 IUsersManagementService userManagementService)
        {
            this._logger = logger;
            this._userManagementService = userManagementService;
        }

        /// <summary>
        /// Verifica se l'email specificata esiste già nel DB o meno
        /// </summary>
        /// <param name="email"></param>
        /// <returns>Ritorna true se l'email non è stta ancora registrata, false se esiste già un utente con l'email specificata</returns>
        [HttpGet("checkemail")]
        [AllowAnonymous]
        public async Task<IActionResult> CheckEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
            {
                return BadRequest();
            }
            var user = await _userManagementService.GetUserByMailAsync(email);
            return Ok(user == null);
        }
    }
}

