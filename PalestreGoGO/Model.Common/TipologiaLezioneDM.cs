using System;
using System.ComponentModel.DataAnnotations;

namespace ready2do.model.common
{
    public partial class TipologiaLezioneDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        [Required]
        public virtual string Nome { get; set; }
        public string Descrizione { get; set; }
        [Required]
        public int Durata { get; set; }
        public int? MaxPartecipanti { get; set; }
        public short? LimiteCancellazioneMinuti { get; set; }
        public short Livello { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public decimal? Prezzo { get; set; }
    }
}
