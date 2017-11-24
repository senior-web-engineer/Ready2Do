using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class AbbonamentiUtenti: BaseMultitenantEntity
    {
        [Required]
        [StringLength(450)]
        public string UserId { get; set; }
        public int IdTipoAbbonamento { get; set; }
        [Column(TypeName = "date")]
        public DateTime DataInizioValidita { get; set; }
        [Column(TypeName = "date")]
        public DateTime Scadenza { get; set; }
        public short? IngressiResidui { get; set; }
        [Column(TypeName = "date")]
        public DateTime? ScadenzaCertificato { get; set; }
        public byte? StatoPagamento { get; set; }

        [ForeignKey("IdCliente")]
        [InverseProperty("AbbonamentiUtenti")]
        public Clienti IdClienteNavigation { get; set; }
        [ForeignKey("IdTipoAbbonamento")]
        [InverseProperty("AbbonamentiUtenti")]
        public TipologieAbbonamenti IdTipoAbbonamentoNavigation { get; set; }
    }
}
