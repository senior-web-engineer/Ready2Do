using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class TakeAppuntamentoResult
    {
        public TipoAppuntamentoPreso TipoAppuntamento { get; set; }
        public int Id { get; set; }
    }

    public enum TipoAppuntamentoPreso
    {
        AppuntamentoConfermato = 1,
        AppuntamentoDaConfermare = 2,
        WaitingList = 3
    }
}
