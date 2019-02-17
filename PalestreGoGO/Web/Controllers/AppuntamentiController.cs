using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Web.Filters;

namespace Web.Controllers
{
    [ServiceFilter(typeof(ReauthenticationRequiredFilter))]
    public class AppuntamentiController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }

    }
}