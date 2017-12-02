using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess.Interfaces
{
    public interface IAbbonamentiRepository
    {
        Task<int> AddAbbonamentoAsync(int idCliente, AbbonamentiUtenti abbonamento);
        Task UpdateAbbonamentoAsync(int idCliente, AbbonamentiUtenti abbonamento);
        IEnumerable<AbbonamentiUtenti> GetAbbonamentiForUser(int idCliente, Guid userId);
        Task<AbbonamentiUtenti> GetAbbonamentoAsync(int idCliente, int idAbbonamento);

    }
}
