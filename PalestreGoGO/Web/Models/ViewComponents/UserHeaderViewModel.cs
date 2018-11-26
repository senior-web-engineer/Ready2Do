using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class UserHeaderViewModel
    {
        public int IdCliente { get; set; }
        public string IdUtente { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string DisplayName { get; set; }

        public string Email { get; set; }
        public string NumTelefono { get; set; }
        public DateTime DataAssociazione { get; set; }
        public ClienteUtenteStatoViewModel Stato { get; set; }
    }
}
