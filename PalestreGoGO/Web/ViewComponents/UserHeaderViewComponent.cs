using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Utils;

namespace Web.ViewComponents
{
    public class UserHeaderViewComponent : ViewComponent
    {

        ILogger<UserHeaderViewComponent> _logger;

        public UserHeaderViewComponent(ILogger<UserHeaderViewComponent> logger)
        {
            _logger = logger;
        }

        public IViewComponentResult Invoke(UserHeaderViewModel model)
        {
            return View(model);
        }
    }
}
