using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class TipologieLezioni: BaseMultitenantEntity
    {
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        [StringLength(500)]
        public string Descrizione { get; set; }
        public int Durata { get; set; }
        public int? MaxPartecipanti { get; set; }
        public short? LimiteCancellazioneMinuti { get; set; }
        public short Livello { get; set; }

        [ForeignKey("IdCliente")]
        [InverseProperty("TipologieLezioni")]
        public Clienti IdClienteNavigation { get; set; }
    }
}
