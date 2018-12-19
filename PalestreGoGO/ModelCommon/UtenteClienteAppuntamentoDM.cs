using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    [Obsolete("Utilizzare AppuntamentoDM oppure AppuntamentoDaConfermareDM")]
    public class UtenteClienteAppuntamentoDM: AppuntamentoBaseDM
    {
        public int IdAbbonamento { get; set; }
        public string Nominativo { get; set; }
        public string Note { get; set; }

        public ScheduleDM Schedule { get; set; }
    }
}
