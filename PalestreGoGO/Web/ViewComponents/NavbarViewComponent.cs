using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
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
            vm.UserDisplayName = UserClaimsPrincipal.DisplayName();
            vm.UserEmail = UserClaimsPrincipal.Email();
            vm.IdClienteCorrente = idClienteCorrente;
            vm.UserIsAuthenticated = User.Identity.IsAuthenticated;
            vm.ReturnUrl = HttpContext.Request.GetDisplayUrl();
            UserType userType4Cliente = UserType.Anonymous;
            if (vm.UserIsAuthenticated)
            {
                var accessToken = await HttpContext.GetTokenAsync("access_token");
                string userId = UserClaimsPrincipal.UserId();
                vm.Notifiche = await _apiClient.GetNotificheForUserAsync(accessToken);
                if (idClienteCorrente.HasValue)
                {
                    userType4Cliente = UserClaimsPrincipal.GetUserTypeForCliente(idClienteCorrente.Value);
                    switch (userType4Cliente)
                    {
                        case UserType.Owner:
                        case UserType.Admin:
                        case UserType.GlobalAdmin:
                            vm.UserCanFollow = false;
                            break;
                        default:
                            vm.UserCanFollow = true;
                            break;
                    }
                    vm.UserIsFollowingCliente = await _apiClient.ClienteIsFollowedByUserAsync(idClienteCorrente.Value, accessToken);
                }
            }
            return View(vm);
        }
    }
}
