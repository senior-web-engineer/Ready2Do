using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class AppuntamentoViewModel
    {
        public int? IdAppuntamento { get; set; }

        public int IdEvento { get; set; }

        public string Nome { get; set; }
        /// <summary>
        /// Data ed ora inizio evento in formato ISO YYYY-MM-DDTHH:MM:SSZ
        /// </summary>
        public string DataOra { get; set; }

        public int DurataMinuti { get; set; }

        public string NomeSala { get; set; }
        public string Istruttore { get; set; }
        /// <summary>
        /// Data ed ora fino a cui è possibile effettuare la cancellazione (con rimborso) in formato ISO YYYY-MM-DDTHH:MM:SSZ
        /// </summary>
        public string MaxDataOraCancellazione { get; set; }

        public int PostiResidui { get; set; }

        public int PostiDisponibili { get; set; }

        /// <summary>
        /// DataOra eventuale iscrizione in formato ISO YYYY-MM-DDTHH:MM:SSZ
        /// </summary>
        public string DataOraIscrizione { get; set; }

    }
}
