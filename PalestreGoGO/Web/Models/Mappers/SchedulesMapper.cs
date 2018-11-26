using PalestreGoGo.WebAPIModel;
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
    }
}
