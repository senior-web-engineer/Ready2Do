using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class LocationViewModel
    {
        public int? Id { get; set; }

        public string Nome { get; set; }

        public string Descrizione { get; set; }

        public short? CapienzaMax { get; set; }
    }
}
