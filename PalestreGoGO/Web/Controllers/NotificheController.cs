using Microsoft.AspNetCore.Authentication.OpenIdConnect;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Filters;

namespace Web.Controllers
{
    [Authorize(AuthenticationSchemes = Constants.OpenIdConnectAuthenticationScheme)]
    [ServiceFilter(typeof(ReauthenticationRequiredFilter))]

    public class NotificheController: Controller
    {
        [HttpGet]
        public IActionResult GetNotificheViewComponent()
        {
            return ViewComponent("NotificheViewComponent");
        }
    }
}
