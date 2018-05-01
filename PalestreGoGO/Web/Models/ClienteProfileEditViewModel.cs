using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ClienteProfileEditViewModel
    {
        public class GalleryEditViewModel
        {
            public GalleryEditViewModel()
            {
                Images = new List<ImageViewModel>();
            }

            public List<ImageViewModel> Images { get; set; }

            public string ContainerUrl { get; set; }
            public string SASToken { get; set; }

        }

        public ClienteProfileEditViewModel()
        {
            //GalleryVM = new GalleryEditViewModel();
            OrarioAperturaVM = new OrarioAperturaViewModel();
        }

        //public GalleryEditViewModel GalleryVM { get; set; }
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
