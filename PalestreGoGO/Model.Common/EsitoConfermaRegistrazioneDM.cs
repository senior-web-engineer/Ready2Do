using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class EsitoConfermaRegistrazioneDM
    {
        public EsitoConfermaRegistrazioneDM()
        {
            EsitoConferma = false;
        }
        public bool EsitoConferma { get; set; }
        public int? IdCliente { get; set; }
        public int? IdRefereer { get; set; }
    }
}
