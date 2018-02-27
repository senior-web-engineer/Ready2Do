using System;
using System.Collections.Generic;
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
            GalleryVM = new GalleryEditViewModel();
            OrarioAperturaVM = new OrarioAperturaViewModel();
        }

        public GalleryEditViewModel GalleryVM { get; set; }
        
        public OrarioAperturaViewModel OrarioAperturaVM { get; set; }
    }
}
