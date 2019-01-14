using ready2do.model.common;
using System.Collections.Generic;
using System.Linq;
using System;

namespace Web.Models.Mappers
{
    public static class SchedulesMapper
    {

        public static SchedulerEventViewModel[] MapToSchedulerEventViewModel(this IEnumerable<ScheduleDM> schedules)
        {
            return schedules.Select(s => new SchedulerEventViewModel()
            {
                Id = s.Id,
                Start = s.DataOraInizio.ToString("O"),
                End = s.DataOraInizio.AddMinutes(s.TipologiaLezione.Durata).ToString("O"),
                Title = s.Title
            }).ToArray();
        }

        public static ScheduleEditViewModel ToVM(this ScheduleDM schedule)
        {
            if (schedule == null) return null;
            return new ScheduleEditViewModel()
            {
                Data = schedule.DataOraInizio.Date,
                OraInizio = schedule.DataOraInizio.TimeOfDay,
                DataCancellazioneMax = schedule.CancellabileFinoAl?.Date,
                OraCancellazioneMax = schedule.CancellabileFinoAl?.TimeOfDay,
                DataChiusuraIscrizioni = schedule.DataChiusuraIscrizione?.Date,
                OraChiusuraIscrizioni = schedule.DataChiusuraIscrizione?.TimeOfDay,
                Id = schedule.Id,
                IdLocation = schedule.IdLocation,
                IdTipoLezione = schedule.IdTipoLezione,
                Istruttore = schedule.Istruttore,
                Note = schedule.Note,
                PostiDisponibili = schedule.PostiDisponibili,
                Title = schedule.Title,
                CancellazioneConsentita = schedule.CancellazioneConsentita,
                DataAperturaIscrizioni = schedule.DataAperturaIscrizione?.Date,
                OraAperturaIscrizioni = schedule.DataAperturaIscrizione?.TimeOfDay,
                PostiResidui = schedule.PostiResidui.Value,
                Recurrency = schedule.Recurrency,
                TipoSchedule = schedule.TipoSchedule,
                VisibileDalDate = schedule.VisibileDal?.Date,
                VisibileDalTime = schedule.VisibileDal?.TimeOfDay,
                VisibileFinoAlDate = schedule.VisibileFinoAl?.Date,
                VisibileFinoAlTime = schedule.VisibileFinoAl?.TimeOfDay,
                WaitListDisponibile  = schedule.WaitListDisponibile
            };
        }

        public static ScheduleInputDM ToApiModel(this ScheduleInputViewModel s, int idCliente)
        {
            ScheduleInputDM result = new ScheduleInputDM();
            result.CancellabileFinoAl = (s.DataCancellazioneMax.HasValue && s.OraCancellazioneMax.HasValue) ? (DateTime?) s.DataCancellazioneMax.Value.Add(s.OraCancellazioneMax.Value) : null;
            result.CancellazioneConsentita = s.CancellazioneConsentita;
            result.DataAperturaIscrizione = (s.DataAperturaIscrizioni.HasValue && s.OraAperturaIscrizioni.HasValue) ? (DateTime?)s.DataAperturaIscrizioni.Value.Add(s.OraAperturaIscrizioni.Value) : null;
            result.DataChiusuraIscrizione = (s.DataChiusuraIscrizioni.HasValue && s.OraChiusuraIscrizioni.HasValue) ? (DateTime?)s.DataChiusuraIscrizioni.Value.Add(s.OraChiusuraIscrizioni.Value) : null;
            result.DataOraInizio = s.Data.Value.Add(s.OraInizio.Value);
            result.Id = s.Id;
            result.IdCliente = idCliente;
            result.IdLocation = s.IdLocation.Value;
            result.IdTipoLezione = s.IdTipoLezione.Value;
            result.Istruttore = s.Istruttore;
            result.Note = s.Note;
            result.PostiDisponibili = s.PostiDisponibili;
            result.Recurrency = s.Recurrency;
            result.TipoSchedule = s.TipoSchedule;
            result.Title = s.Title;
            result.VisibileDal = (s.VisibileDalDate.HasValue && s.VisibileDalTime.HasValue) ? (DateTime?)s.VisibileDalDate.Value.Add(s.VisibileDalTime.Value) : null;
            result.VisibileFinoAl = (s.VisibileFinoAlDate.HasValue && s.VisibileFinoAlTime.HasValue) ? (DateTime?)s.VisibileFinoAlDate.Value.Add(s.VisibileFinoAlTime.Value) : null;
            result.WaitListDisponibile = s.WaitListDisponibile;
            return result;
        }
    }
}
