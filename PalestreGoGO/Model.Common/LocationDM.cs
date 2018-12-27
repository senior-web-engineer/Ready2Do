using System;

namespace ready2do.model.common
{

    public class LocationDM: LocationInputDM
    {
        public DateTime DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
    }
}
