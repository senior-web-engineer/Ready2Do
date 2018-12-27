using System;

namespace ready2do.model.common
{
    public class TipologiaClienteDM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public DateTime? DataCancellazione { get; set; }
    }
}
