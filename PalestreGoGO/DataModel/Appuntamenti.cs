using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class Appuntamenti : BaseMultitenantEntity
    {
        [Required]
        public string UserId { get; set; }
        public int ScheduleId { get; set; }
        public DateTime DataPrenotazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        [StringLength(1000)]
        public string Note { get; set; }
        [StringLength(200)]
        public string Nominativo { get; set; }
        public bool IsGuest { get; set; }

        [ForeignKey("ScheduleId")]
        [InverseProperty("Appuntamenti")]
        public Schedules Schedule { get; set; }
    }
}
