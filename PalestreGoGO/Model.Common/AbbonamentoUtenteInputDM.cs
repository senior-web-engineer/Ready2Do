using System;

namespace ready2do.model.common
{
    public class AbbonamentoUtenteInputDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string UserId { get; set; }
        public int IdTipoAbbonamento { get; set; }
        public DateTime DataInizioValidita { get; set; }
        public DateTime Scadenza { get; set; }
        public short? IngressiIniziali { get; set; }
        public short? IngressiResidui { get; set; }

        public decimal? Importo { get; set; }
        public decimal? ImportoPagato { get; set; }

    }
}
