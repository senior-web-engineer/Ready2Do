using ready2do.model.common;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ITipologieClientiRepository 
    {
        Task<IEnumerable<TipologiaClienteDM>> GetAllAsync(bool includeDeleted = false);

    }
}
