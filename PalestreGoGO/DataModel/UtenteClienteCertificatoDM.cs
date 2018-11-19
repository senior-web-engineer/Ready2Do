using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class UtenteClienteCertificatoDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public UserReferenceDM User { get; set; }
        public DateTime DataPresentazione { get; set; }
        public DateTime DataScadenza { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public string Note { get; set; }
       
    }
}
