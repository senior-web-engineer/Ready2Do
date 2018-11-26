using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class UtenteViewCertificatiViewModel
    {
        public IList<CertificatUtenteViewModel> Certificati{ get; set; }

        public string UserId { get; set; }
        public int IdCliente { get; set; }
    }
}
