using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.ViewComponents
{
    public class NavbarViewModel
    {
        public int? IdClienteCorrente { get; set; }
        public string UserDisplayName { get; set; }
        public string UserEmail { get; set; }
        public bool UserIsAuthenticated { get; set; }
        public int NumNotifichePresenti { get { return Notifiche?.Count ?? 0; } }
        public int NumNuovNotifichePresenti { get { return Notifiche?.Count(n => n.IsNew) ?? 0; } }
        public List<NotificaViewModel> Notifiche { get; set; }
        public string ReturnUrl { get; set; }
        public bool? UserIsFollowingCliente { get; set; }
    }
}
