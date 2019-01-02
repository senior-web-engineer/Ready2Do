using ready2do.model.common;
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
            Images = new List<ImmagineClienteDM>();
        }

        public string ContainerUrl { get; set; }

        public List<ImmagineClienteDM> Images { get; set; }
    }
}
