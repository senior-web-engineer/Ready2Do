using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class TipologieAbbonamenti : BaseMultitenantEntity
    {
        public TipologieAbbonamenti()
        {
            AbbonamentiUtenti = new HashSet<AbbonamentiUtenti>();
        }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        public short? DurataMesi { get; set; }
        public short? NumIngressi { get; set; }
        [Column(TypeName = "decimal(10, 2)")]
        public decimal? Costo { get; set; }
        public short? MaxLivCorsi { get; set; }

        [ForeignKey("IdCliente")]
        [InverseProperty("TipologieAbbonamenti")]
        public Clienti IdClienteNavigation { get; set; }
        [InverseProperty("IdTipoAbbonamentoNavigation")]
        public ICollection<AbbonamentiUtenti> AbbonamentiUtenti { get; set; }
    }
}
