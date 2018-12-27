using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ILocationsRepository 
    {
        Task<IEnumerable<LocationDM>> GetAllAsync(int idCliente, bool includeDeleted = false);
        Task<LocationDM> GetSingleAsync(int idCliente, int idLocation, bool includeDeleted = false);
        Task<int> AddAsync(int idCliente, LocationInputDM location);
        Task UpdateAsync(int idCliente, LocationInputDM location);
        Task Delete(int idCliente, int idLocation);
    }
}
