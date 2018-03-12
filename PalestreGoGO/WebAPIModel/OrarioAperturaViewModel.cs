using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class FasciaOrariaViewmodel
    {
        public TimeSpan Inizio { get; set; }
        public TimeSpan Fine { get; set; }
    }

    public class GiornoViewModel
    {
        public GiornoViewModel()
        {
            Mattina = new FasciaOrariaViewmodel();
            Pomeriggio = new FasciaOrariaViewmodel();
            IsContinuato = true;
        }

        public FasciaOrariaViewmodel Mattina { get; set; }
        public FasciaOrariaViewmodel Pomeriggio { get; set; }
        public bool IsContinuato { get; set; }
        public bool IsChiuso { get; set; }
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
