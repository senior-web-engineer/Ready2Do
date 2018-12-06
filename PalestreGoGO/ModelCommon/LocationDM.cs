using System;

namespace ready2do.model.common
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
