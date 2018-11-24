using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class CertificatUtenteViewModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string UserId { get; set; }
        public DateTime DataPresentazione { get; set; }
        public DateTime DataScadenza { get; set; }
        public string Note { get; set; }
    }
}
