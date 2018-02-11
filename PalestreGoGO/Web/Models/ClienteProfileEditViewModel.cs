using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ClienteProfileEditViewModel
    {
        public ClienteProfileEditViewModel()
        {
            GalleryImages = new List<ImageViewModel>();
        }

        public string SASToken { get; set; }
        
        public List<ImageViewModel> GalleryImages {get;set;}
    }
}
