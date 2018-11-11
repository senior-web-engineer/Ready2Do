using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AbbonamentoUtenteViewModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string UserId { get; set; }
        public TipologiaAbbonamentoViewModel TipoAbbonamento { get; set; }
        public DateTime DataInizioValidita { get; set; }
        public DateTime? Scadenza { get; set; }
        public int? IngressiResidui { get; set; }
        public DateTime? ScadenzaCertificato { get; set; }
        public byte? StatoPagamento { get; set; }
    }
}
