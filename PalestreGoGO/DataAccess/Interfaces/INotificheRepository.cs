using PalestreGoGo.DataModel;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess.Interfaces
{
    public interface INotificheRepository
    {
        Task<long> AddNotificaAsync(NotificaDM notifica);

        Task<IEnumerable<NotificaConTipoDM>> GetNotificheAsync(UserReferenceDM userRef, int? idCliente = null, FiltroListaNotificheDM filtro = FiltroListaNotificheDM.SoloAttive);

        Task UpdateNotifica(long idNotifica, DateTime? dataView, DateTime? dataDismiss);
    }
}
