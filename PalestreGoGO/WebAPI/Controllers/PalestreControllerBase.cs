using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.Controllers
{
    public class PalestreControllerBase: ControllerBase
    {
        protected ClaimsPrincipal GetCurrentUser()
        {
            return this.User;
        }
    }
}
