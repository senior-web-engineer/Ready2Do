using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class AbbonamentoUtenteApiModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public Guid UserId { get; set; }
        public TipologieAbbonamentiViewModel TipoAbbonamento { get; set; }
        public int? IdTipoAbbonamento { get; set; }
        public DateTime DataInizioValidita { get; set; }
        public DateTime? Scadenza { get; set; }
        public int? IngressiResidui { get; set; }
        public DateTime? ScadenzaCertificato { get; set; }
        public byte? StatoPagamento { get; set; }
    }
}
