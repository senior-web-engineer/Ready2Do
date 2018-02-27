using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ImageViewModel
    {
        public int? Id { get; set; }
        public string Url { get; set; }
        public string Alt { get; set; }
        public string Caption { get; set; }
        public int Ordinamento { get; set; }
    }
}
