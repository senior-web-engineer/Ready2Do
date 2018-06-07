using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ClienteProfileInputModel
    {     
        public ClienteProfileInputModel()
        {
            OrarioAperturaVM = new OrarioAperturaViewModel();
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

        [Required]
        public string Descrizione { get; set; }

        #region Indirizzo
        public string Indirizzo { get; set; }

        public bool EsitoLookup { get; set; }

        [Required(ErrorMessage = "Indirizzo non valido")]
        public string Latitudine { get; set; }

        [Required(ErrorMessage = "Indirizzo non valido")]
        public string Longitudine { get; set; }

        [Required(ErrorMessage = "Indirizzo non valido")]
        public string Citta { get; set; }

        public string Country { get; set; }

        public string CAP { get; set; }
        #endregion

        [Required]
        public ImageViewModel ImmagineHome { get; set; }
        
        
        public OrarioAperturaViewModel OrarioAperturaVM { get; set; }
    }
}
