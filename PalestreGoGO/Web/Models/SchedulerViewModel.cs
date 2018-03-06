using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class SchedulerViewModel
    {
        public IEnumerable<LocationViewModel> Sale { get; set; }

        public int? IdActiveLocation { get; set; }            
    }
}
