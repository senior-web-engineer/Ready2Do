using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Models
{
    public class ClienteRegistrazioneInputModel
    {
        public string URL { get; set; }

        public string NomeStruttura { get; set; }

        public short IdTipologia { get; set; }

        public string RagioneSociale { get; set; }

        public string EmailStruttura { get; set; }

        //[Required]
        //[MaxLength(255)]
        //[GoogleAdrressValidation("EsitoLookup","E' necessario selezionare un indirizzo tra quelli proposti nella lista")]
        public string Indirizzo { get; set; }

        public string Telefono { get; set; }

        public string ReturnUrl { get; set; }

        public short EsitoLookup { get; set; }

        [Required(ErrorMessage ="Indirizzo non valido")]
        public string Latitudine { get; set; }

        [Required(ErrorMessage = "Indirizzo non valido")]
        public string Longitudine { get; set; }

        [Required(ErrorMessage = "Indirizzo non valido")]
        public string Citta { get; set; }

        public string Country { get; set; }

        public string CAP { get; set; }

        // Aggiunto per testare il passaggio delle coordinate come stringa prodotta dierettamente dalla libreria Google invece di avere problemi con il separatori decimali
        public string Coordinate { get; set; }
    }
}
