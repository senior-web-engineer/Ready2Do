using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ScheduleViewViewModel
    {
        public ScheduleInputViewModel Schedule { get; set; }

        public List<UtenteClienteDM> UtentiRegistrati { get; set; }


    }
}
