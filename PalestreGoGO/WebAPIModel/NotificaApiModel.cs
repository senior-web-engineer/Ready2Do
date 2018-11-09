using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class NotificaApiModel
    {
        public long? Id { get; set; }
        public UserReferenceApiModel UserRef { get; set; }
        public int IdTipo { get; set; }
        public int? IdCliente { get; set; }
        public string Titolo { get; set; }
        public string Testo { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime? DataDismissione { get; set; }
        public DateTime? DataPrimaVisualizzazione { get; set; }
    }
}
