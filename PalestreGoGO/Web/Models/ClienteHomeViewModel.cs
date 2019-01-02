using ready2do.model.common;
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
            Images = new List<ImmagineClienteDM>();
        }

        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string RagioneSociale { get; set; }
        public string Email { get; set; }
        public string NumTelefono { get; set; }
        public string Descrizione { get; set; }

        public string Indrizzo { get; set; }

        public OrarioAperturaViewModel OrarioApertura { get; set; }

        public ImmagineClienteDM ImmagineHome { get; set; }

        public List<ImmagineClienteDM> Images { get; set; }

        public List<LocationDM> Locations { get; set; }

        public string GoogleStaticMapUrl { get; set; }
        public string ExternalGoogleMapUrl { get; set; }

        public string EventsBaseUrl { get; set; }

        public float Latitude { get; set; }
        public float Longitude { get; set; }

        public string DataMinima { get; set; }
        public string DataMassima { get; set; }

    }
}
