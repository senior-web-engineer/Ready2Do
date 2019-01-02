using Microsoft.AspNetCore.Mvc.Rendering;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    //public enum TipoOrarioViewModel
    //{
    //    Spezzato = 1,
    //    Continuato = 2,
    //    Mattina = 3,
    //    Pomeriggio = 4,
    //    Chiuso = 5
    //}

    //public class FasciaOrariaViewmodel
    //{
    //    [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")]
    //    public string Inizio { get; set; }
    //    [RegularExpression("^([0-9]|0[0-9]|1[0-9]|2[0-3]):[0-5][0-9]$")]
    //    public string Fine { get; set; }
    //}

    //public class GiornoViewModel
    //{
    //    public GiornoViewModel()
    //    {
    //        Mattina = new FasciaOrariaViewmodel();
    //        Pomeriggio = new FasciaOrariaViewmodel();
    //        TipoOrario = TipoOrarioViewModel.Spezzato;
    //    }

    //    public GiornoViewModel(string nome) : base()
    //    {
    //        Nome = nome;
    //    }

    //    public string Nome { get; set; }
    //    public FasciaOrariaViewmodel Mattina { get; set; }
    //    public FasciaOrariaViewmodel Pomeriggio { get; set; }
    //    public TipoOrarioViewModel TipoOrario { get; set; }

    //    public override string ToString()
    //    {
    //        switch (TipoOrario)
    //        {
    //            case TipoOrarioViewModel.Chiuso:
    //                return "Chiuso";
    //            case TipoOrarioViewModel.Continuato:
    //                return string.Format("{0} - {1}", Mattina.Inizio, Pomeriggio.Fine);
    //            case TipoOrarioViewModel.Mattina:
    //                return string.Format("{0} - {1}", Mattina.Inizio, Mattina.Fine);
    //            case TipoOrarioViewModel.Pomeriggio:
    //                return string.Format("{0} - {1}", Pomeriggio.Inizio, Pomeriggio.Fine);
    //            case TipoOrarioViewModel.Spezzato:
    //                return string.Format("{0} - {1} , {2} - {3}", Mattina.Inizio, Mattina.Fine, Pomeriggio.Inizio, Pomeriggio.Fine);
    //            default:
    //                return "";
    //        }
    //    }
    //}

    public class OrarioAperturaViewModel
    {

        public OrarioAperturaViewModel()
        {
            LunVen = new GiornoAperturaDM();
            Sabato = new GiornoAperturaDM(); //new GiornoViewModel("Sabato");
            Domenica = new GiornoAperturaDM(); //new GiornoViewModel("Domenica");
        }

        //public GiornoViewModel Lunedi { get; set; }
        //public GiornoViewModel Martedi { get; set; }
        //public GiornoViewModel Mercoledi { get; set; }
        //public GiornoViewModel Giovedi { get; set; }
        //public GiornoViewModel Venerdi { get; set; }
        public GiornoAperturaDM LunVen { get; set; }
        public GiornoAperturaDM Sabato { get; set; }
        public GiornoAperturaDM Domenica { get; set; }

        public List<SelectListItem> TipiFasceOrarie { get; } = new List<SelectListItem>()
        {
            new SelectListItem("Spezzato", "1"),
            new SelectListItem("Continuato" , "2"),
            new SelectListItem("Mattina" , "3"),
            new SelectListItem("Pomeriggio" , "4"),
            new SelectListItem("Chiuso" , "5")
        };
    }
}
