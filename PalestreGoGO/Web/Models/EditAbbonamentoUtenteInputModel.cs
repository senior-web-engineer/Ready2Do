using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class EditAbbonamentoUtenteInputModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public Guid IdUtente { get; set; }
        public int IdTipologiaAbbonamento { get; set; }
        public DateTime DataInizioValidita { get; set; }
        public DateTime? Scadenza { get; set; }
        public int? IngressiResidui { get; set; }
        public DateTime? ScadenzaCertificato { get; set; }
        public bool Pagato { get; set; }

    }
}
