using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Filters
{
    public class OwnersRedirectToConfirmFilter : IResourceFilter
    {
        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //Nothing to do after execution
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated) { return; }
            //Se è owner di una struttura ma non ha l'email confermata => redirect
            if ((context.HttpContext.User.StruttureOwned().Count() > 0) &&
                (!context.HttpContext.User.EmailConfirmedOn().HasValue))
            {
                context.Result = new RedirectToRouteResult("MailToConfirmRoute");
            }
        }
    }
}
