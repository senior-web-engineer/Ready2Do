using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class ScheduleDetailsViewModel: ScheduleViewModel
    {
        public TipologieLezioniViewModel TipologiaLezione { get; set; }
        public LocationViewModel Location { get; set; }
    }
}
