using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.Utils
{
    public static class SchedulesMapper
    {

        public static SchedulerEventViewModel[] MapToSchedulerEventViewModel(this IEnumerable<ScheduleDetailsViewModel> schedules) {
            return schedules.Select(s => new SchedulerEventViewModel()
            {
                Id = s.Id,
                Start = s.Data.Add(s.OraInizio).ToString("O"),
                End = s.Data.Add(s.OraInizio).AddMinutes(s.TipologiaLezione.Durata).ToString("O"),
                Title = s.Title
            }).ToArray();
        }
    }
}
