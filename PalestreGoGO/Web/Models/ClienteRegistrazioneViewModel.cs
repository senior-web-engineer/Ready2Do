using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Models
{
    public class ClienteRegistrazioneViewModel
    {
        public string URL { get; set; }

        public string NomeStruttura { get; set; }

        public short IdTipologia { get; set; }

        public string RagioneSociale { get; set; }

        public string EmailStruttura { get; set; }

        public string Indirizzo { get; set; }

        public string Telefono { get; set; }

        public string ReturnUrl { get; set; }

        public short EsitoLookup { get; set; }

        public string Citta { get; set; }

        public string Country { get; set; }

        public string CAP { get; set; }

        /// <summary>
        /// Coordinate ritornate dal servizio di geocoding nel formato:
        /// (lat, long) 
        /// Dove:
        ///   lat e long sono due float serializzati con il punto come separatore decimale (xx.xxx)
        /// </summary>
        public string Coordinate { get; set; }
    }
}
