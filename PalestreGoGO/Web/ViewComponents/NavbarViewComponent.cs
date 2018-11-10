using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models.ViewComponents;
using Web.Utils;

namespace Web.ViewComponents
{
    public class NavbarViewComponent : ViewComponent
    {
        private readonly WebAPIClient _apiClient;
        private readonly ILogger<NavbarViewComponent> _logger;

        public NavbarViewComponent(WebAPIClient apiClient,
                                       ILogger<NavbarViewComponent> logger)
        {
            _apiClient = apiClient;
            _logger = logger;
        }

        public async Task<IViewComponentResult> InvokeAsync(int? idClienteCorrente)
        {
            NavbarViewModel vm = new NavbarViewModel();
            vm.IdClienteCorrente = idClienteCorrente;
            vm.UserIsAuthenticated = User.Identity.IsAuthenticated;
            vm.ReturnUrl = HttpContext.Request.GetDisplayUrl();
            if (vm.UserIsAuthenticated)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                vm.Notifiche = await _apiClient.GetNotificheForUserAsync(accessToken);
            }
            return View(vm);
        }
    }
}
