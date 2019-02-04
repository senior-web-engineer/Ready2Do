using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Models
{
    public class ClientRegistrationStrutturaInputModel
    {
        /* DATI UTENTE*/
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(100)]
        public string Cognome { get; set; }
        public string DisplayName { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        [Remote("CheckEmail","Account")]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Conferma Password")]
        public string PasswordConfirm { get; set; }

        [Required]
        [Display(Name = "Url struttura")]
        [Remote("CheckUrl", "Clienti")]
        [MinLength(3)]
        public string URL { get; set; }

        /* DATI STRUTTURA */
        [Required]
        [MaxLength(100)]
        public string NomeStruttura { get; set; }

        [Display(Name = "Categoria")]
        public short IdTipologia { get; set; }

        [Required]
        [MaxLength(100)]
        [Display(Name ="Ragione Sociale")]
        public string RagioneSociale { get; set; }

        [Required]
        [EmailAddress]
        [MaxLength(256)]
        [Display(Name ="Email Struttura")]
        public string EmailStruttura { get; set; }

        [Required]
        [MaxLength(255)]
        [GoogleAdrressValidation("EsitoLookup","E' necessario selezionare un indirizzo tra quelli proposti nella lista")]
        public string Indirizzo { get; set; }

        [Phone]
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
    }
}
