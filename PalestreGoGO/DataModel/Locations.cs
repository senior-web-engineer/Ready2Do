using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class Locations : BaseMultitenantEntity
    {
        public Locations()
        {
            Schedules = new HashSet<Schedules>();
        }

        [Required]
        [StringLength(100)]
        public string Nome { get; set; }

        public string Descrizione { get; set; }
        public short? CapienzaMax { get; set; }

        [ForeignKey("IdCliente")]
        [InverseProperty("Locations")]
        public Clienti IdClienteNavigation { get; set; }
        [InverseProperty("Location")]
        public ICollection<Schedules> Schedules { get; set; }
    }
}
