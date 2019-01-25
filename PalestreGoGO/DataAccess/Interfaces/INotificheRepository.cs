using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface INotificheRepository
    {
        Task<long> AddNotificaAsync(NotificaDM notifica);

        Task<IEnumerable<NotificaConTipoDM>> GetNotificheAsync(string userId, int? idCliente = null, FiltroListaNotificheDM filtro = FiltroListaNotificheDM.SoloAttive, int pageNumber = 1, int pageSize = 10);

        /// <summary>
        /// Aggiorna la notifica
        /// </summary>
        /// <param name="userId">Utente chiamante (serve per verificare che la notifica sia effettivamente la sua)</param>
        /// <param name="idNotifica"></param>
        /// <param name="dataView"></param>
        /// <param name="dataDismiss"></param>
        /// <returns></returns>
        Task UpdateNotifica(string userId, long idNotifica, DateTime? dataView, DateTime? dataDismiss);
    }
}
