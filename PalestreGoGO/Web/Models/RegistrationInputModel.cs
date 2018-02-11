﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class RegistrationInputModel
    {
        /* DATI UTENTE*/
        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(100)]
        public string Cognome { get; set; }
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }
        [Required]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Conferma Password")]
        public string PasswordConfirm { get; set; }

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
        public string Indirizzo { get; set; }

        [Phone]
        public string Telefono { get; set; }

        public string ReturnUrl { get; set; }

        public bool EsitoLookup { get; set; }
        public float? Latitudine { get; set; }
        public float? Longitudine { get; set; }
        public string Citta { get; set; }
        public string Country { get; set; }
        public string CAP { get; set; }
    }
}
