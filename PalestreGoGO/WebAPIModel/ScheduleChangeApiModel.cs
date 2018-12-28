using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class ScheduleChangeApiModel
    {
        public ScheduleInputDM Schedule { get; set; }
        public TipoModificaScheduleDM TipoModifica { get; set; }
    }
}
