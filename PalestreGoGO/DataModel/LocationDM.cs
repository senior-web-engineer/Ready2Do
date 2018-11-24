using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class LocationDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public short? CapienzaMax { get; set; }

    }
}
