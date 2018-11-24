using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    [Obsolete]
    public class ClienteUtenteConAbbonamento
    {
        public int IdCliente { get; set; }
        public string UserId { get; set; }
        public string Nominativo { get; set; }
        public string DisplayName { get; set; }
        public DateTime DataAssociazione { get; set; }
        public AbbonamentiUtenti Abbonamento { get; set; }
    }
}
