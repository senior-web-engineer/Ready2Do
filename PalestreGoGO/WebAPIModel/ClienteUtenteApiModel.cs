using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    /// <summary>
    /// Versione di un UtenteCliente con i dati proveniente da B2C
    /// integrati con i dati presenti a Sistema (Es: DataAssociazione, Stato, ecc.)
    /// </summary>
    public class ClienteUtenteDetailsApiModel
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
        public ClienteUtenteStato Stato { get; set; }
        
    }
}
