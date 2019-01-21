using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ready2do.model.common
{

    public class AppuntamentoDM : AppuntamentoBaseDM
    {
        public int? IdAbbonamento { get; set; }

        [StringLength(1000)]
        public string Note { get; set; }

        [StringLength(200)]
        public string Nominativo { get; set; }

        public UtenteClienteDM User { get; set; }
    }
}
