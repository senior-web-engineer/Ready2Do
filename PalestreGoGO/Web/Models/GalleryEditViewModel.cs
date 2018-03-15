using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class GalleryEditViewModel
    {
        public GalleryEditViewModel()
        {
            Images = new List<ImageViewModel>();
        }

        public string ContainerUrl { get; set; }

        public List<ImageViewModel> Images { get; set; }
    }
}
