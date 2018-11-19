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
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string DisplayName { get; set; }
        public string Email { get; set; }
        public string TelephoneNumber { get; set; }
        public DateTime UtlimoAggiornamento { get; set; }
        public DateTime DataAssociazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public ClienteUtenteStatoApiModel Stato { get; set; }
        
    }
}
