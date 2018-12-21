using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{

    public abstract class EsitoAppuntamentoBaseDM
    {

    }
    /// <summary>
    /// Rappresenta l'esito di un'operazione di "Presa Appuntamento".
    /// </summary>
    public class EsitoAppuntamentoOKDM : EsitoAppuntamentoBaseDM
    {
        public int IdAppuntamento { get; set; }
        public bool DaConfermare { get; set; }
    }

    public class EsitoAppuntamentoKODM : EsitoAppuntamentoBaseDM
    {
        public int CodErrore { get; set; }
        public string DescErrro { get; set; }
    }

}
