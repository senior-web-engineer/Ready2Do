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

        WebAPIClient _apiClient;
        ILogger<UserHeaderViewComponent> _logger;

        public UserHeaderViewComponent(WebAPIClient apiClient,
                                       ILogger<UserHeaderViewComponent> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public IViewComponentResult Invoke(UserHeaderViewModel model)
        {
            return View(model);
        }
    }
}
