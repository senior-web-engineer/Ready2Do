﻿using PalestreGoGo.DataModel;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PalestreGoGo.DataAccess
{
    public interface INotificheRepository
    {
        Task<long> AddNotificaAsync(NotificaDM notifica);

        Task<IEnumerable<NotificaConTipoDM>> GetNotificheAsync(string userId, int? idCliente = null, FiltroListaNotificheDM filtro = FiltroListaNotificheDM.SoloAttive);

        Task UpdateNotifica(long idNotifica, DateTime? dataView, DateTime? dataDismiss);
    }
}
