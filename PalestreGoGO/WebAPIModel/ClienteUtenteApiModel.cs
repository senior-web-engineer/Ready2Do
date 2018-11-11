using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class ClienteUtenteApiModel
    {
        public int IdCliente { get; set; }
        public string IdUtente { get; set; }
        public string Nominativo { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }

        public DateTime DataAssociazione { get; set; }

    }
}
