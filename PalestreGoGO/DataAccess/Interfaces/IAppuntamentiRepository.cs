using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess.Interfaces
{
    public interface IAppuntamentiRepository
    {
        Task<int> AddAppuntamentoAsync(int idCliente, Appuntamenti appuntamento);

        Task CancelAppuntamentoAsync(int idCliente, int idAppuntamento);

        IEnumerable<Appuntamenti> GetAppuntamentiForSchedule(int idCliente, int idSchedule);

        Task<Appuntamenti> GetAppuntamentoAsync(int idCliente, int idAppuntamento);
    }
}
