using Microsoft.AspNetCore.Mvc.Razor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Utils
{
    public class SharedViewsLocationExpander : IViewLocationExpander
    {
        public IEnumerable<string> ExpandViewLocations(ViewLocationExpanderContext context, IEnumerable<string> viewLocations)
        {
            return viewLocations
                    .Append("/Views/Shared/Site/{0}.cshtml")
                    .Append("/Views/Shared/Clienti/{0}.cshtml");
        }

        public void PopulateValues(ViewLocationExpanderContext context)
        {
            //Nothing to do here
            return;
        }
    }
}
