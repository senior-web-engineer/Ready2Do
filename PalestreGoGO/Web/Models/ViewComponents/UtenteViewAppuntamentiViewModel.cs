using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class UtenteViewAppuntamentiViewModel
    {
        public IList<AppuntamentoUtenteViewModel> Appuntamenti{ get; set; }

        public string UserId { get; set; }
        public int IdCliente { get; set; }
    }
}
