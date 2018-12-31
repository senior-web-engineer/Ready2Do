using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ready2do.model.common
{
    public class AbbonamentoUtenteDM : AbbonamentoUtenteInputDM
    {
 
        public DateTime? DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }

        public TipologiaAbbonamentoDM TipoAbbonamento { get; set; }

    }
}
