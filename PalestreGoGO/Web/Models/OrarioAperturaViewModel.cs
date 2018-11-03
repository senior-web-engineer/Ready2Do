﻿using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
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
            TipoOrario = TipoOrarioViewModel.Spezzato;
        }

        public GiornoViewModel(string nome) : base()
        {
            Nome = nome;
        }

        public string Nome { get; set; }
        public FasciaOrariaViewmodel Mattina { get; set; }
        public FasciaOrariaViewmodel Pomeriggio { get; set; }
        public TipoOrarioViewModel TipoOrario { get; set; }
    }

    public class OrarioAperturaViewModel
    {

        public OrarioAperturaViewModel()
        {
            //Lunedi = new GiornoViewModel("Lunedì");
            //Martedi = new GiornoViewModel("Martedì");
            //Mercoledi = new GiornoViewModel("Mercoledì");
            //Giovedi = new GiornoViewModel("Giovedì");
            //Venerdi = new GiornoViewModel("Venerdì");
            LunVen = new GiornoViewModel("Venerdì");
            Sabato = new GiornoViewModel("Sabato");
            Domenica = new GiornoViewModel("Domenica");
        }

        //public GiornoViewModel Lunedi { get; set; }
        //public GiornoViewModel Martedi { get; set; }
        //public GiornoViewModel Mercoledi { get; set; }
        //public GiornoViewModel Giovedi { get; set; }
        //public GiornoViewModel Venerdi { get; set; }
        public GiornoViewModel LunVen { get; set; }
        public GiornoViewModel Sabato { get; set; }
        public GiornoViewModel Domenica { get; set; }

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
