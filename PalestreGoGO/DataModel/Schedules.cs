using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class Schedules : BaseMultitenantEntity
    {
        public Schedules()
        {
            Appuntamenti = new HashSet<Appuntamenti>();
        }
        [Required]
        [StringLength(100)]
        public string Title { get; set; }

        public int IdTipoLezione { get; set; }
        public int IdLocation { get; set; }
        [Column(TypeName = "date")]
        public DateTime Data { get; set; }
        public TimeSpan OraInizio { get; set; }
        [StringLength(150)]
        public string Istruttore { get; set; }
        public int PostiDisponibili { get; set; }
        public int? PostiResidui { get; set; }

        public DateTime CancellabileFinoAl { get; set; }
        public DateTime DataChiusuraIscrizione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        [StringLength(450)]
        public string UserIdOwner { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        //[Column("timestamp")]
        //public byte[] Timestamp { get; set; }

        [ForeignKey("IdCliente")]
        [InverseProperty("Schedules")]
        public Clienti Cliente { get; set; }

        [ForeignKey("IdLocation")]
        [InverseProperty("Schedules")]
        public Locations Location { get; set; }

        [InverseProperty("Schedule")]
        public ICollection<Appuntamenti> Appuntamenti { get; set; }

        [ForeignKey("IdTipoLezione")]
        [InverseProperty("Schedules")]
        public TipologieLezioni TipologiaLezione { get; set; }

    }
}
