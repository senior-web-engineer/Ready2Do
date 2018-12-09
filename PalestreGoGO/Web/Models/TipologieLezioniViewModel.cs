using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class TipologieLezioniViewModel 
    {
        public List<SelectListItem> IntervalliTempo { get; } = new List<SelectListItem>()
        {
                 new SelectListItem("00:15", "15"),
                 new SelectListItem("00:30", "30"),
                 new SelectListItem("00:45", "45"),
                 new SelectListItem("01:00", "60"),
                 new SelectListItem("01:30", "90"),
                 new SelectListItem("02:00", "120"),
                 new SelectListItem("02:30", "150"),
                 new SelectListItem("03:00", "180"),
                 new SelectListItem("04:00", "240"),
                 new SelectListItem("05:00", "300"),
                 new SelectListItem("06:00", "360"),
                 new SelectListItem("12:00", "720"),
                 new SelectListItem("24:00", "1440"),
                 new SelectListItem("48:00", "2880")
        };

        public int? Id { get; set; }
        public int IdCliente { get; set; }

        [Required]
        [StringLength(100)]
        [Remote("CheckNome", "TipologieLezioni",AdditionalFields ="IdCliente, Id")]
        public string Nome { get; set; }

        [StringLength(500)]
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
