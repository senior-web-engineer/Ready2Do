using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ISchedulesRepository
    {
        Task<int> AddScheduleAsync(int idCliente, ScheduleDM schedule);
        Task UpdateScheduleAsync(int idCliente, ScheduleDM schedule);
        Task<ScheduleDM> GetScheduleAsync(int idCliente, int idSchedule);
        Task<IEnumerable<ScheduleDM>> GetScheduleListAsync(int idCliente, DateTime? startDate = null, DateTime? endDate = null, int? idLocation = null, int? idTipoLezione = null, 
                                                            bool soloPostiDisp = false, bool soloIscrizAperte = false, int pageSize = 25, int pageNumber = 1, 
                                                            string sortColumn = "dataorainizio", bool ascending = true, bool includeDeleted= false);
        Task RemoveScheduleAsync(int idCliente, int idSchedule);

        //IEnumerable<Schedules> GetSchedules(int idCliente, DateTime startDate, DateTime endDate);
        Task<IEnumerable<Schedules>> GetSchedulesAsync(int idCliente, DateTime? startDate = null, DateTime? endDate = null, int? idLocation = null);
    }
}
