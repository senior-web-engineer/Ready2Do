using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Filters
{
    public class OwnersRedirectToConfirmFilter : IResourceFilter
    {
        private IConfiguration _configuration;

        public OwnersRedirectToConfirmFilter(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void OnResourceExecuted(ResourceExecutedContext context)
        {
            //Nothing to do after execution
        }

        public void OnResourceExecuting(ResourceExecutingContext context)
        {
            if (!context.HttpContext.User.Identity.IsAuthenticated) { return; }
            var actDescriptor = context.ActionDescriptor as ControllerActionDescriptor;
            if (actDescriptor != null) {
                //Escludiamo tutte le pagine del controlloer Home
                if (actDescriptor.ControllerName.Equals("Home")) { return; }
                //Escludiamo la pagina di conferma email ed il reinvio dal redirect
                if(actDescriptor.ControllerName.Equals("Account", StringComparison.InvariantCultureIgnoreCase) &&
                    (actDescriptor.ActionName.Equals("MailToConfirm", StringComparison.InvariantCultureIgnoreCase) ||
                     actDescriptor.ActionName.Equals("SendNewConfirmMail", StringComparison.InvariantCultureIgnoreCase)))
                {
                    return;
                }
                //Se è owner di una struttura ma non ha l'email confermata => redirect
                if ((context.HttpContext.User.StruttureOwned().Count() > 0) &&
                (!context.HttpContext.User.EmailConfirmedOn().HasValue))
                {
                    context.Result = new RedirectToActionResult("MailToConfirm", "Account", null);
                }
            }
        }
    }
}
