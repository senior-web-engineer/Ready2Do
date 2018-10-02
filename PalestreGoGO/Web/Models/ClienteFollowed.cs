using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ClienteFollowed
    {
        [Required]
        public int IdCliente { get; set; }

        public string RagioneSociale { get; set; }

        public string NomeCliente { get; set; } 

        [Required]
        public DateTime DataFollowing { get; set; }

        [Required]
        public bool HasAbbonamentoValido { get; set; }
    }
}
