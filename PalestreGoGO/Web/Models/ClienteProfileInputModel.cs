using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

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

        //[Required]
        public string Descrizione { get; set; }

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

        [Required]
        public ImmagineClienteDM ImmagineHome { get; set; }
        
        
        public OrarioAperturaViewModel OrarioAperturaVM { get; set; }
    }
}
