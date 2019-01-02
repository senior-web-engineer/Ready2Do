using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class SchedulerViewModel
    {
        public IEnumerable<LocationDM> Sale { get; set; }

        public int? IdActiveLocation { get; set; }            

        public TimeSpan? MinTime { get; set; }
        public TimeSpan? MaxTime { get; set; }
    }
}
