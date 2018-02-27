using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class ClientiImmagini : BaseMultitenantEntity
    {
        public int IdTipoImmagine { get; set; }
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        [StringLength(100)]
        public string Alt { get; set; }

        [Required]
        [StringLength(1000)]
        public string Url { get; set; }
        [StringLength(1000)]
        public string Descrizione { get; set; }

        public int Ordinamento { get; set; }

        [ForeignKey("IdCliente")]
        [InverseProperty("ClientiImmagini")]
        public Clienti IdClienteNavigation { get; set; }

        [ForeignKey("IdTipoImmagine")]
        [InverseProperty("ClientiImmagini")]
        public TipologieImmagini IdTipoImmagineNavigation { get; set; }
    }
}
