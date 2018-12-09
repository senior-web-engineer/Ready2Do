using PalestreGoGo.WebAPIModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Mappers
{
    public static class SchedulesMapper
    {

        public static SchedulerEventViewModel[] MapToSchedulerEventViewModel(this IEnumerable<ScheduleDetailedApiModel> schedules) {
            return schedules.Select(s => new SchedulerEventViewModel()
            {
                Id = s.Id,
                Start = s.DataOraInizio.ToString("O"),
                End = s.DataOraInizio.AddMinutes(s.TipologiaLezione.Durata).ToString("O"),
                Title = s.Title                
            }).ToArray();
        }

        public static ScheduleViewModel ToVM(this ScheduleDM schedule)
        {
            if (schedule == null) return null;
            return new ScheduleViewModel()
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

            };
        }
    }
}
