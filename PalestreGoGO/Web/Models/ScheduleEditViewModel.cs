using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ScheduleEditViewModel : ScheduleInputViewModel
    {
        public List<SelectListItem> TipologieSchedule { get; } = new List<SelectListItem>()
        {
            new SelectListItem("Solo per Abbonati", "1"),
            new SelectListItem("GRATUITO solo per Abbonati", "10"),
            new SelectListItem("GRATUITO solo per NON Abbonati", "11"),
            new SelectListItem("GRATUITO per TUTTI)", "20")
        };

        public ScheduleEditViewModel()
        {

        }

        public ScheduleEditViewModel(ScheduleInputViewModel schedIn)
        {
            this.CancellazioneConsentita = schedIn.CancellazioneConsentita;
            this.Data = schedIn.Data;
            this.DataAperturaIscrizioni = schedIn.DataAperturaIscrizioni;
            this.DataCancellazioneMax = schedIn.DataCancellazioneMax;
            this.DataChiusuraIscrizioni = schedIn.DataChiusuraIscrizioni;
            this.Id = schedIn.Id;
            this.IdLocation = schedIn.IdLocation;
            this.IdTipoLezione = schedIn.IdTipoLezione;
            this.Istruttore = schedIn.Istruttore;
            this.Note = schedIn.Note;
            this.OraAperturaIscrizioni = schedIn.OraAperturaIscrizioni;
            this.OraCancellazioneMax = schedIn.OraCancellazioneMax;
            this.OraChiusuraIscrizioni = schedIn.OraChiusuraIscrizioni;
            this.OraInizio = schedIn.OraInizio;
            this.PostiDisponibili = schedIn.PostiDisponibili;
            this.PostiResidui = schedIn.PostiResidui;
            this.Recurrency = schedIn.Recurrency;
            this.TipoSchedule = schedIn.TipoSchedule;
            this.Title = schedIn.Title;
            this.VisibileDalDate = schedIn.VisibileDalDate;
            this.VisibileDalTime = schedIn.VisibileDalTime;
            this.VisibileFinoAlDate = schedIn.VisibileFinoAlDate;
            this.VisibileFinoAlTime = schedIn.VisibileFinoAlTime;
            this.WaitListDisponibile = schedIn.WaitListDisponibile;
        }
    }
}
