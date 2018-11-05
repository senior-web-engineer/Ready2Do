using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.ViewComponents
{
    public class NotificheViewComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
            /**/
            return View();
        }

    }
}
