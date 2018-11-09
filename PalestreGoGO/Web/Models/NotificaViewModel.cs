using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class NotificaViewModel
    {
        public long Id { get; set; }
        public string IconName{ get; set; }
        public string Titolo { get; set; }
        public string Testo { get; set; }
        public DateTime DataCreazione { get; set; }
        public bool IsNew { get; set; }
    }
}
