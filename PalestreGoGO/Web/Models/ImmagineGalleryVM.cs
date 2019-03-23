using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ImmagineGalleryVM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public int? IdTipoImmagine { get; set; }
        public string Nome { get; set; }
        public string Alt { get; set; }
        /// <summary>
        /// L'url ha senso SOLO se non è una nuova immagine (Image = NULL)
        /// </summary>
        public string Url { get; set; }
        /// <summary>
        /// Valorizzato se viene selezionata un'immagine di cui fare l'upload
        /// </summary>
        public IFormFile Image { get; set; }
        public string ThumbnailUrl { get; set; }
        public string Descrizione { get; set; }
        public int Ordinamento { get; set; }
    }
}
