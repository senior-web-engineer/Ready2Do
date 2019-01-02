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
        Task<int> AddScheduleAsync(int idCliente, ScheduleInputDM schedule);
        Task UpdateScheduleAsync(int idCliente, ScheduleInputDM schedule, TipoModificaScheduleDM tipoModifica);
        Task<ScheduleDM> GetScheduleAsync(int idCliente, int idSchedule, bool includeDeleted = false);

        /// <summary>
        /// Questo metodo non è destinato ad essere esposto direttamente dalle API, rappresenterebbe infatti un problema di sicurezza non essendoci un filtro sul cliente.
        /// Deve essere utilizzato internamente alle API per fare il lookup degli Schedules a partire degli Id.
        /// Nasce originariamente per integrare, nelle API, i dati degli appuntamenti con quelli dei relativi schedule e ritornare un oggeto composito con entrambe le informazioni
        /// mantenendo però separati id 2 data layer
        /// </summary>
        /// <param name="idSchedules"></param>
        /// <param name="includeDeleted"></param>
        /// <returns></returns>
        Task<IEnumerable<ScheduleDM>> SchedulesLookupAsync(IEnumerable<int> idSchedules, bool includeDeleted = false);

        Task<IEnumerable<ScheduleDM>> GetScheduleListAsync(int idCliente, DateTime? startDate = null, DateTime? endDate = null, int? idLocation = null, int? idTipoLezione = null, 
                                                            bool soloPostiDisp = false, bool soloIscrizAperte = false, int pageSize = 25, int pageNumber = 1, 
                                                            string sortColumn = "dataorainizio", bool ascending = true, bool includeDeleted= false);
        Task DeleteScheduleAsync(int idCliente, int idSchedule);
    }
}
