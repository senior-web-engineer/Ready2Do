using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI
{
    public static class ApiConfigs
    {
        public static GraphAPIOptions GraphAPIOptions { get; private set; }

        public static void InitConfiguration(IConfiguration configuration)
        {
            var graphAPIOptions = new GraphAPIOptions();
            configuration.Bind("Authentication:GraphAPI", graphAPIOptions);
            GraphAPIOptions = graphAPIOptions;
        }
    }
}
