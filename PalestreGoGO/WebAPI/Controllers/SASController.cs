using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PalestreGoGo.WebAPI.ViewModel;

namespace WebAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/SAS")]
    [Authorize]
    public class SASController : ControllerBase
    {

        private const string CUSTOM_HEADER_TOKEN_SAS = "X-SASToken";

        //public async Task<IActionResult> BuildToken()
        //{
        //    //TODO: Implementare la logica 
        //}

        [HttpGet]
        //Accesso anonimo ma verifichiamo l'header custom
        [AllowAnonymous]
        public async Task<IActionResult> GetBlobSAS([FromQuery]string _method, [FromQuery]string bloburi, [FromQuery] string qqtimestamp)
        {

            if (!HttpContext.Request.Headers.ContainsKey(CUSTOM_HEADER_TOKEN_SAS))
            {
                return BadRequest();
            }
            var header = HttpContext.Request.Headers[CUSTOM_HEADER_TOKEN_SAS];
            var token = header.FirstOrDefault();
            if (string.IsNullOrWhiteSpace(token))
            {
                return Unauthorized();
            }
            //Decode TOKEN

            return Ok();
        }


        //private SASTokenModel internalDecodeToken(string token)
        //{

        //}
    }
}