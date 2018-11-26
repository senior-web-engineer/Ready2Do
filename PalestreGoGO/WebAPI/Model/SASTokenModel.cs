using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel
{
    public class SASTokenModel
    {
        public string SecurityToken { get; set; }
        public DateTime CreatedOn { get; set; }
        public string Container { get; set; }

    }
}
