using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class TipologieClienti : BaseEntity
    {
        public TipologieClienti()
        {
            Clienti = new HashSet<Clienti>();
        }

        [Required]
        [StringLength(50)]
        public string Nome { get; set; }
        [StringLength(100)]
        public string Descrizione { get; set; }

        [InverseProperty("IdTipologiaNavigation")]
        public ICollection<Clienti> Clienti { get; set; }
    }
}
