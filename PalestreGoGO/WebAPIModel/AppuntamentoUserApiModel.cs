using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class AppuntamentoUserApiModel
    {
        public int IdAppuntamento { get; set; }

        public int IdCliente { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public int IdAbbonamento { get; set; }
        public string Nominativo { get; set; }
        public string Note { get; set; }

        public ScheduleDetailedApiModel Schedule { get; set; }
    }
}
