using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class UtenteViewAbbonamentiViewModel
    {
        public IList<AbbonamentoUtenteViewModel> Abbonamenti { get; set; }

        public string UserId { get; set; }
        public int IdCliente { get; set; }
    }
}
