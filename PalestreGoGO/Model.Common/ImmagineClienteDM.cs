using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{

    public class ImmagineClienteDM: ImmagineClienteInputDM
    {
        public DateTime DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }

    }


}
