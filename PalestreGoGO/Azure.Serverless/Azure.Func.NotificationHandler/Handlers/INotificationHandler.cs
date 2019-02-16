using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FuncNotificationHandler.Handlers
{
    public interface INotificationHandler
    {
        Task HandleNotificationAsync();
    }
}
