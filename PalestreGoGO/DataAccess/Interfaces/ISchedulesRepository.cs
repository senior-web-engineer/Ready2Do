using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ISchedulesRepository
    {
        Task<int> AddScheduleAsync(int idCliente, ScheduleDM schedule);
        Task UpdateScheduleAsync(int idCliente, ScheduleDM schedule, TipoModificaScheduleDM tipoModifica);
        Task<ScheduleDM> GetScheduleAsync(int idCliente, int idSchedule, bool includeDeleted = false);
        Task<IEnumerable<ScheduleDM>> GetScheduleListAsync(int idCliente, DateTime? startDate = null, DateTime? endDate = null, int? idLocation = null, int? idTipoLezione = null, 
                                                            bool soloPostiDisp = false, bool soloIscrizAperte = false, int pageSize = 25, int pageNumber = 1, 
                                                            string sortColumn = "dataorainizio", bool ascending = true, bool includeDeleted= false);
        Task DeleteScheduleAsync(int idCliente, int idSchedule);
    }
}
