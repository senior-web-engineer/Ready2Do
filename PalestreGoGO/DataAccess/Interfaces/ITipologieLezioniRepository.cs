using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ITipologieLezioniRepository //: IBaseRepository<TipologieLezioni, int>
    {
        Task<IEnumerable<TipologieLezioni>> GetListAsync(int idTenant, string sortColumn = null, bool sortAsc = true, int pageNumber = 1, int pageSize = 1000);
        Task<int> CountAsync(int idTenant);
        Task<TipologieLezioni> GetAsync(int idTenant, int itemKey);

        Task<int> AddAsync(int idTenant, TipologieLezioni entity);
        Task UpdateAsync(int idTenant, TipologieLezioni entity);
        Task DeleteAsync(int idTenant, int itemKey);
    }
}
