using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class TipologiaAbbonamentoDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public short? DurataMesi { get; set; }
        public short? NumIngressi { get; set; }
        public decimal? Costo { get; set; }
        public short? MaxLivCorsi { get; set; }
        public DateTime ValidoDal { get; set; }
        public DateTime? ValidoFinoAl { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
    }
}
