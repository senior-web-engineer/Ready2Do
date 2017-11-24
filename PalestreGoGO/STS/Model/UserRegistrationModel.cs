using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Palestregogo.STS.Model
{
    public class UserRegistrationModel
    {
        [Required]
        [EmailAddress]
        [MaxLength(256)]
        public string Email { get; set; }

        [Required]
        [MaxLength(100)]
        public string Nome { get; set; }
        [Required]
        [MaxLength(100)]
        public string Cognome { get; set; }

        [Phone]
        public string Telefono { get; set; }

        [Required]
        public string Password { get; set; }

        [Required]
        public string CorrelationTokoen { get; set; }
    }
}
