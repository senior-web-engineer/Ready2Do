using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class AppuntamentoWithScheduleDM : AppuntamentoDM
    {
        public ScheduleDM Schedule { get; set; }
    }
}
