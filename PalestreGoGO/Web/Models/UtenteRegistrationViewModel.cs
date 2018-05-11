using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class UtenteRegistrationViewModel
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
        //Validazione per assicurarsi che l'email non sia già registrata
        [Remote("CheckEmail","Account")]
        public string Email { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string PasswordConfirm { get; set; }
    }
}
