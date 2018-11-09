using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = OpenIdConnectDefaults.AuthenticationScheme)]
    public class NotificheController: Controller
    {
        [HttpGet]
        public IActionResult GetNotificheViewComponent()
        {
            return ViewComponent("NotificheViewComponent");
        }
    }
}
