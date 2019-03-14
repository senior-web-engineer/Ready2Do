using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Web.Utils
{
    public static class RouteUtils
    {
        //public static string

        public static string GetCurrentUrl(this HttpRequest request)
        {
            StringBuilder sb = new StringBuilder();
            if (request.PathBase.HasValue)
                sb.AppendFormat("{0}/{1}", request.PathBase, request.Path);
            else
                sb.AppendFormat("{0}", request.Path);
            if (request.QueryString.HasValue)
                sb.AppendFormat("?{0}", request.QueryString.Value);

            return sb.ToString();
        }
    }
}
