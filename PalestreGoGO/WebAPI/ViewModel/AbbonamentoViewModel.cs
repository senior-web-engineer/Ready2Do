using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel
{
    [Obsolete]
    public class AbbonamentoViewModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public Guid UserId { get; set; }
        public int IdTipoAbbonamento { get; set; }
        public DateTime DataInizioValidita { get; set; }
        public DateTime Scadenza { get; set; }
        public short? IngressiResidui { get; set; }
        public DateTime? ScadenzaCertificato { get; set; }
        public byte? StatoPagamento { get; set; }
    }
}
