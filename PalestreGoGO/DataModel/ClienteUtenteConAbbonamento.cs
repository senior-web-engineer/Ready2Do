using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class ClienteUtenteConAbbonamento
    {
        public int IdCliente { get; set; }
        public Guid IdUtente { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }
        public DateTime DataAssociazione { get; set; }

        public AbbonamentiUtenti Abbonamento { get; set; }
    }
}
