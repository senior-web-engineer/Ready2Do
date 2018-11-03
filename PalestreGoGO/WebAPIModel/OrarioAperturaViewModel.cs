using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public enum TipoOrarioViewModel
    {
        Spezzato = 1,
        Continuato = 2,
        Mattina = 3,
        Pomeriggio = 4,
        Chiuso = 5
    }

    public class FasciaOrariaViewmodel
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public TimeSpan? Inizio { get; set; }

        [JsonProperty(NullValueHandling = NullValueHandling.Include)]
        public TimeSpan? Fine { get; set; }
    }

    public class GiornoViewModel
    {
        public GiornoViewModel()
        {
            Mattina = new FasciaOrariaViewmodel();
            Pomeriggio = new FasciaOrariaViewmodel();
            TipoOrario = TipoOrarioViewModel.Spezzato;
        }

        public FasciaOrariaViewmodel Mattina { get; set; }
        public FasciaOrariaViewmodel Pomeriggio { get; set; }
        public TipoOrarioViewModel TipoOrario { get; set; }

    }

    public class OrarioAperturaViewModel
    {
        public OrarioAperturaViewModel()
        {
            Lunedi = new GiornoViewModel();
            Martedi = new GiornoViewModel();
            Mercoledi = new GiornoViewModel();
            Giovedi = new GiornoViewModel();
            Venerdi = new GiornoViewModel();
            Sabato = new GiornoViewModel();
            Domenica = new GiornoViewModel();
        }

        public GiornoViewModel Lunedi { get; set; }
        public GiornoViewModel Martedi { get; set; }
        public GiornoViewModel Mercoledi { get; set; }
        public GiornoViewModel Giovedi { get; set; }
        public GiornoViewModel Venerdi { get; set; }
        public GiornoViewModel Sabato { get; set; }
        public GiornoViewModel Domenica { get; set; }

    }
}
