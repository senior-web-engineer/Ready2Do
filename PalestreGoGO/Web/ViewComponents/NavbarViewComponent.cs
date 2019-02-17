using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Filters;
using Web.Models;
using Web.Models.Mappers;
using Web.Models.ViewComponents;
using Web.Proxies;
using Web.Utils;

namespace Web.ViewComponents
{
    [ServiceFilter(typeof(ReauthenticationRequiredFilter))]
    public class NavbarViewComponent : ViewComponent
    {
        private readonly UtentiProxy _utentiProxy;
        private readonly ILogger<NavbarViewComponent> _logger;

        public NavbarViewComponent(UtentiProxy utentiProxy,
                                   ILogger<NavbarViewComponent> logger)
        {
            _utentiProxy = utentiProxy;
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
                string userId = UserClaimsPrincipal.UserId();
                vm.Notifiche = (await _utentiProxy.GetNotificheForUserAsync()).MapToViewModel();
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
                    vm.UserIsFollowingCliente = await _utentiProxy.ClienteIsFollowedByUserAsync(idClienteCorrente.Value);
                }
            }
            return View(vm);
        }
    }
}
