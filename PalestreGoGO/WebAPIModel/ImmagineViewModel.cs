using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class ImmagineViewModel
    {
        public int? Id { get; set; }
        public string Url { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public string Alt { get; set; }
        public int Ordinamento { get; set; }
    }
}
