using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class FasciaOrariaViewmodel
    {
        public TimeSpan Inizio { get; set; }
        public TimeSpan Fine { get; set; }
    }

    public class GiornoApertura
    {
        public GiornoApertura()
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
            Lunedi = new GiornoApertura();
            Martedi = new GiornoApertura();
            Mercoledi = new GiornoApertura();
            Giovedi = new GiornoApertura();
            Venerdi = new GiornoApertura();
            Sabato = new GiornoApertura();
            Domenica = new GiornoApertura();
        }

        public GiornoApertura Lunedi { get; set; }
        public GiornoApertura Martedi { get; set; }
        public GiornoApertura Mercoledi { get; set; }
        public GiornoApertura Giovedi { get; set; }
        public GiornoApertura Venerdi { get; set; }
        public GiornoApertura Sabato { get; set; }
        public GiornoApertura Domenica { get; set; }

    }
}
