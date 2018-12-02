using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class UtenteClienteAbbonamentoDM
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

        public DateTime? DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }

        public TipoAbbonamentoDM TipoAbbonamento { get; set; }

    }
}
