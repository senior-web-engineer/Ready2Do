using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class TipologieLezioniViewModel
    {
        public int? Id { get; set; }

        [Required]
        public string Nome { get; set; }
        public string Descrizione { get; set; }

        [Required]
        public int Durata { get; set; }
        public int? MaxPartecipanti { get; set; }
        public short? LimiteCancellazioneMinuti { get; set; }
        public short Livello { get; set; }
    }
}
