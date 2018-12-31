using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IAbbonamentiRepository
    {
        Task<int> SaveAbbonamentoAsync(int idCliente, AbbonamentoUtenteInputDM abbonamento);
        Task DeleteAbbonamentoAsync(int idCliente, string userId, int idAbbonamento);
        Task<IEnumerable<AbbonamentoUtenteDM>> GetAbbonamentiForUserAsync(int idCliente, string userId, bool includeExpired, bool includeDeleted, int? idEvento = null);
        Task<AbbonamentoUtenteDM> GetAbbonamentoAsync(int idCliente, int idAbbonamento);

    }
}
