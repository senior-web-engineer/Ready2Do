using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class TipoAbbonamentoDM
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public short? DurataMesi { get; set; }
        public short? NumIngressi { get; set; }
        public decimal? Costo { get; set; }
        public short? MaxLivCorsi { get; set; }
    }
}
