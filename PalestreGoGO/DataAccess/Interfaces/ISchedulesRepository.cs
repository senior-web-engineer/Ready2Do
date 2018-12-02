using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ISchedulesRepository
    {
        Task<int> SaveScheduleAsync(int idCliente, ScheduleDM schedule);

        Task RemoveScheduleAsync(int idCliente, int idSchedule);

        Task<Schedules> GetScheduleAsync(int idCliente, int idSchedule);

        //IEnumerable<Schedules> GetSchedules(int idCliente, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Schedules>> GetSchedulesAsync(int idCliente, DateTime? startDate = null, DateTime? endDate = null, int? idLocation = null);
    }
}
