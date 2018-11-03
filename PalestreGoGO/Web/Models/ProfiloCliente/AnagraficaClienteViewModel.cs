using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Models
{
    public class AnagraficaClienteViewModel
    {     
        public AnagraficaClienteViewModel()
        {

        }

        [Required]
        public int IdCliente { get; set; }

        [Required]
        public string Nome { get; set; }

        [Required]
        public string RagioneSociale { get; set; }

        [Required]
        public string Email { get; set; }

        [Required]
        public string NumTelefono { get; set; }

        //[Required]
        public string Descrizione { get; set; }

        [Required]
        [Remote(action: "CheckUrl", controller: "Clienti", AdditionalFields = "IdCliente")]
        [MinLength(3)]
        public string UrlRoute { get; set; }

        #region Indirizzo
        [GoogleAdrressValidation("EsitoLookup", "E' necessario selezionare un indirizzo tra quelli proposti nella lista")]
        [Required]
        public string Indirizzo { get; set; }

        public short EsitoLookup { get; set; }

        [Required(ErrorMessage = "Indirizzo non valido")]
        public string Latitudine { get; set; }

        [Required(ErrorMessage = "Indirizzo non valido")]
        public string Longitudine { get; set; }

        [Required(ErrorMessage = "Indirizzo non valido")]
        public string Citta { get; set; }

        public string Country { get; set; }

        public string CAP { get; set; }
        #endregion
        
    }
}
