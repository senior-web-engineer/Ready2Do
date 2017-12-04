using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Utils;
using Microsoft.AspNetCore.Authorization;

namespace Web.Controllers
{
    [Authorize]
    public class ClientiController : Controller
    {

        public ClientiController()
        {

        }

        [HttpGet("{id}")]
        //[AllowAnonymous]
        public IActionResult Index([FromRoute(Name ="id")]int idCliente)
        {
            //var cliente = WebAPIClient.GetClienteAsync()
            return View();
        }
    }
}