using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ISchedulesRepository
    {
        Task AddScheduleAsync(int idCliente, Schedules schedule);

        Task RemoveScheduleAsync(int idCliente, int idSchedule);

        Task<IEnumerable<Schedules>> GetSchedulesAsync(int idCliente, DateTime startDate, DateTime endDate);
        
    }
}
