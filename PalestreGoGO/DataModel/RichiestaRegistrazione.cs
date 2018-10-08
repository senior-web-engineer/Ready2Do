using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class RichiestaRegistrazione
    {
        public int Id { get; set; }
        public DateTime DataRichiesta { get; set; }
        public Guid CorrelationId { get; set; }
        public string UserCode { get; set; }
        public string Username { get; set; }
        public DateTime Expiration { get; set; }
        public DateTime DataConferma { get; set; }
    }
}
