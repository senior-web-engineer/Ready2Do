using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface ITipologieLezioniRepository 
    {
        Task<IEnumerable<TipologiaLezioneDM>> GetListAsync(int idTenant, string sortColumn = null, bool sortAsc = true, int pageNumber = 1, int pageSize = 1000, bool includeDeleted = false);
        Task<int> CountAsync(int idTenant);
        Task<TipologiaLezioneDM> GetAsync(int idTenant, int itemKey);

        Task<int> AddAsync(int idTenant, TipologiaLezioneDM entity);
        Task UpdateAsync(int idTenant, TipologiaLezioneDM entity);
        Task DeleteAsync(int idTenant, int itemKey);
        Task<bool> CheckNameAsync(int idTenant, string nome, int? id);
    }
}
