using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class TipologieImmagini : BaseEntity
    {
        public TipologieImmagini()
        {
            ClientiImmagini = new HashSet<ClientiImmagini>();
        }

        [Required]
        [StringLength(10)]
        public string Codice { get; set; }
        [Required]
        [StringLength(100)]
        public string Nome { get; set; }
        [Required]
        [StringLength(500)]
        public string Descrizione { get; set; }
        [Column(TypeName = "datetime2(2)")]
        public DateTime DataCreazione { get; set; }

        [InverseProperty("IdTipoImmagineNavigation")]
        public ICollection<ClientiImmagini> ClientiImmagini { get; set; }
    }
}
