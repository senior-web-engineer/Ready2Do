using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ScheduleViewModel : ScheduleInputViewModel
    {
        public List<SelectListItem> TipologieSchedule { get; } = new List<SelectListItem>()
        {
            new SelectListItem("Solo per Abbonati", "1"),
            new SelectListItem("GRATUITO solo per Abbonati", "10"),
            new SelectListItem("GRATUITO solo per NON Abbonati", "11"),
            new SelectListItem("GRATUITO per TUTTI)", "20")
        };

            //public ScheduleViewModel()
            //{
            //    DataEventoOptions = new DatePickerOptionJSModel()
            //    {
            //        DefaultDate = DateTime.Now,
            //        MinDate = DateTime.Now,
            //        SetDefaultDate = true
            //    };
            //    DataCancellazioneOptions = new DatePickerOptionJSModel()
            //    {
            //        DefaultDate = DateTime.Now,
            //        MinDate = DateTime.Now,
            //        SetDefaultDate = true
            //    };
            //}

        //public DatePickerOptionJSModel DataEventoOptions { get; set; }
        //public DatePickerOptionJSModel DataCancellazioneOptions { get; set; }
    }
}
