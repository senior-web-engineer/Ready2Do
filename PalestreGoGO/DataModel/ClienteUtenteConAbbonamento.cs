using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class ClienteUtenteConAbbonamento
    {
        public int IdCliente { get; set; }
        public string IdUtente { get; set; }
        public string Nominativo { get; set; }
        public string DisplayName { get; set; }
        public DateTime DataAssociazione { get; set; }
        public AbbonamentiUtenti Abbonamento { get; set; }
    }
}
