using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AppuntamentoUtenteViewModel
    {
        public int Id { get; set; }

        public string Nome { get; set; }

        public string NomeCliente { get; set; }

        public string ClienteUrlRoute { get; set; }

        public int IdEvento { get; set; }
        /// <summary>
        /// Data ed ora inizio evento 
        /// </summary>
        public DateTime DataOra { get; set; }

        /// <summary>
        /// DataOra eventuale iscrizione 
        /// </summary>
        public DateTime DataOraIscrizione { get; set; }

        public DateTime? DataCancellazione { get; set; }

        public bool Cancellabile { get; set; }
    }
}
