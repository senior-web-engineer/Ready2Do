using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncNotificationHandler.Handlers
{
    public abstract class BaseNotificationHandler: INotificationHandler
    {
        protected NotificationMessage _message;

        public BaseNotificationHandler(NotificationMessage message)
        {
            _message = message;
        }

        public abstract Task HandleNotificationAsync();
    }
}
