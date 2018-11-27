using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class AbbonamentoUtenteViewModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string UserId { get; set; }
        [Required]
        public int? IdTipoAbbonamento { get; set; }
        public string NomeTipoAbbonamento { get; set; }
        [Required]
        public DateTime? DataInizioValidita { get; set; }
        [Required]
        public DateTime? Scadenza { get; set; }
        public short? IngressiIniziali { get; set; }
        public short? IngressiResidui { get; set; }
        public decimal? Importo { get; set; }
        public decimal? ImportoPagato { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public DateTime? DataCreazione { get; set; }

        //public IEnumerable<KeyValuePair<int, string>> TipologieAbbonamento { get; set; }
    }
}
