using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public class DataAccessLogMessage
    {
        public int? IdCliente { get; set; }
        public string UserId { get; set; }
        public DateTime? DataMessaggio { get; set; }
        public string Message { get; set; }
        public LogMessageLevel Level { get; set; }
    }
}
