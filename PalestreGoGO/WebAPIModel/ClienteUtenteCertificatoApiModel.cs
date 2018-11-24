using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class ClienteUtenteCertificatoApiModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string UserId { get; set; }
        public DateTime DataPresentazione { get; set; }
        public DateTime DataScadenza { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public string Note { get; set; }

    }
}
