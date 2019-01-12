using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Models;
using Web.Utils;

namespace Web.ViewComponents
{
    
    public class SideNavViewComponent: ViewComponent
    {
        public IViewComponentResult Invoke(int ?idClienteCorrente)
        {
            SideNavViewModel vm = new SideNavViewModel();
            vm.IsVisible = UserClaimsPrincipal.CanViewSidebar(idClienteCorrente.Value);
            if (vm.IsVisible)
            {
                vm.CurrentController = ViewContext.RouteData.Values["Controller"].ToString().ToLowerInvariant();
                vm.CurrentAction = ViewContext.RouteData.Values["Action"].ToString().ToLowerInvariant();
                vm.ClienteRoute = ViewContext.RouteData.Values["Cliente"].ToString();
            }
            return View(vm);
        }
    }
}
