using FuncNotificationHandler.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncNotificationHandler.Handlers
{
    public static class NotificationHandlerFactory
    {
        public static INotificationHandler CreateHandler(NotificationMessage notifica)
        {
            switch (notifica.TipoNotifica.ToUpper())
            {
                case TipologieNotifiche.NUOVO_ACCOUNT:
                    return new NuovoAccountHandler(notifica);
            }
            throw new NotImplementedException($"Unable to handle notification of type {notifica.TipoNotifica} - subtype: {notifica.SubType}");
        }
    }
}
