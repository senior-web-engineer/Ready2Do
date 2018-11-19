using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IAbbonamentiRepository
    {
        Task<int> SaveAbbonamentoAsync(int idCliente, UtenteClienteAbbonamentoDM abbonamento);
        Task DeleteAbbonamentoAsync(int idCliente, string userId, int idAbbonamento);
        Task<IEnumerable<UtenteClienteAbbonamentoDM>> GetAbbonamentiForUserAsync(int idCliente, string userId, bool includeExpired, bool includeDeleted);
        Task<UtenteClienteAbbonamentoDM> GetAbbonamentoAsync(int idCliente, int idAbbonamento);

    }
}
