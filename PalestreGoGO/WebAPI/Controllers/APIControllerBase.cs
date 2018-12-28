using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    public class APIControllerBase: ControllerBase
    {
        [NonAction]
        public virtual ClaimsPrincipal GetCurrentUser()
        {
            return this.User;
        }
    }
}
