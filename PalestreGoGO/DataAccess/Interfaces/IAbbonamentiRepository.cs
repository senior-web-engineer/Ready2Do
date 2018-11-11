using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IAbbonamentiRepository
    {
        Task<int> AddAbbonamentoAsync(int idCliente, AbbonamentiUtenti abbonamento);
        Task UpdateAbbonamentoAsync(int idCliente, AbbonamentiUtenti abbonamento);
        IEnumerable<AbbonamentiUtenti> GetAbbonamentiForUser(int idCliente, string userId);
        Task<AbbonamentiUtenti> GetAbbonamentoAsync(int idCliente, int idAbbonamento);

    }
}
