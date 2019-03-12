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
        public string IdUtente { get; set; }
        public int IdTipoAbbonamento { get; set; }
        public DateTime DataInizioValidita { get; set; }
        public DateTime? Scadenza { get; set; }
        public short? IngressiIniziali { get; set; }
        public short? IngressiResidui { get; set; }
        public decimal Importo { get; set; }
        public decimal ImportoPagato { get; set; }

        public bool? ConfermaAppuntamenti { get; set; }

    }
}
