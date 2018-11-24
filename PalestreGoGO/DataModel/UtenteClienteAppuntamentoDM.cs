using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class UtenteClienteAppuntamentoDM
    {
        public int IdAppuntamento { get; set; }
        public int IdCliente { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public int IdAbbonamento { get; set; }
        public string Nominativo { get; set; }
        public string Note { get; set; }
        public int IdSchedule { get; set; }

        public ScheduleDM Schedule { get; set; }
    }
}
