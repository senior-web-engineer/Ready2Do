using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class TipologiaAbbonamentoDM: TipologiaAbbonamentoInputDM
    {
        public DateTime DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
    }
}
