using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface IAppuntamentiRepository
    {
        Task<int> AddAppuntamentoAsync(int idCliente, Appuntamenti appuntamento);

        Task CancelAppuntamentoAsync(int idCliente, int idAppuntamento);

        IEnumerable<Appuntamenti> GetAppuntamentiForSchedule(int idCliente, int idSchedule);

        Task<Appuntamenti> GetAppuntamentoAsync(int idCliente, int idAppuntamento);

        Task<Appuntamenti> GetAppuntamentoForScheduleAsync(int idCliente, int idSchedule, string userId);

        IEnumerable<Appuntamenti> GetAppuntamentiForUser(int idCliente, string userId, bool includePast = false);

        IEnumerable<Appuntamenti> GetAppuntamentiForUser(string userId, bool includePast = false);
    }
}
