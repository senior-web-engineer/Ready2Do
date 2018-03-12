using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Palestregogo.STS
{
    public class ApplicationConfiguration
    {
        public class WebAPIConfig
        {
            public string BaseAddress { get; set; }
        }

        public string RegistrationUrl { get; set; }
        /// <summary>
        /// Homepage del sito a cui fare il redirect in caso di annullamento dell'operazione di login se il returnUrl non è valido
        /// </summary>
        public string HomeUrl { get; set; }
        WebAPIConfig APIConfig { get; set; }
    }

}
