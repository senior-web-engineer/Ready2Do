using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class FasciaOrariaViewmodel
    {
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")]
        public string Inizio { get; set; }
        [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")]
        public string Fine { get; set; }
    }

    public class GiornoViewModel
    {
        public GiornoViewModel()
        {
            Mattina = new FasciaOrariaViewmodel();
            Pomeriggio = new FasciaOrariaViewmodel();
            IsContinuato = true;
        }

        public GiornoViewModel(string nome) : base()
        {
            Nome = nome;
        }

        public string Nome { get; set; }
        public FasciaOrariaViewmodel Mattina { get; set; }
        public FasciaOrariaViewmodel Pomeriggio { get; set; }
        public bool IsContinuato { get; set; }
        public bool IsChiuso { get; set; }
    }

    public class OrarioAperturaViewModel
    {
        public OrarioAperturaViewModel()
        {
            Lunedi = new GiornoViewModel("Lunedì");
            Martedi = new GiornoViewModel("Martedì");
            Mercoledi = new GiornoViewModel("Mercoledì");
            Giovedi = new GiornoViewModel("Giovedì");
            Venerdi = new GiornoViewModel("Venerdì");
            Sabato = new GiornoViewModel("Sabato");
            Domenica = new GiornoViewModel("Domenica");
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
