using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class ImmagineDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public int IdTipoImmagine { get; set; }
        public string Nome { get; set; }
        public string Alt { get; set; }
        public string Url { get; set; }
        public string Descrizione { get; set; }
        public int Ordinamento { get; set; }

    }
}
