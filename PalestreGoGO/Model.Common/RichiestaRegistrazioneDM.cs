using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class RichiestaRegistrazioneDM
    {
        public int Id { get; set; }
        public DateTime DataRichiesta { get; set; }
        public string CorrelationId { get; set; }
        public string UserCode { get; set; }
        public string Username { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime? DataConferma { get; set; }
        public DateTime? DataCancellazione{ get; set; }
    }
}
