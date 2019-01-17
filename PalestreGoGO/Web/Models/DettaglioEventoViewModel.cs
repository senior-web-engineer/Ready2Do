using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class DettaglioEventoViewModel
    {
        public ScheduleDM Schedule { get; set; }
        public IEnumerable<AppuntamentoDM> Appuntamenti { get; set; }
        public IEnumerable<AppuntamentoDaConfermareDM> AppuntamentiDaConfermare { get; set; }
        public IEnumerable<WaitListRegistration> WaitListRegistrations { get; set; }
    }
}
