using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ClienteHomeViewModel
    {
        public ClienteHomeViewModel()
        {
            Images = new List<ImageViewModel>();
        }

        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string RagioneSociale { get; set; }
        public string Email { get; set; }
        public string NumTelefono { get; set; }
        public string Descrizione { get; set; }
        //public TipologiaClienteViewModel Tipologia { get; set; }
        public string Indrizzo { get; set; }
        public string OrarioApertura { get; set; }
        public ImageViewModel ImmagineHome { get; set; }
        public List<ImageViewModel> Images { get; set; }

    }
}
