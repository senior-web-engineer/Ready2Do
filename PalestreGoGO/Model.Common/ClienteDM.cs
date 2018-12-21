using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class ClienteDM
    {
        public int Id { get; set; }
        public string Nome { get; set; }
        public string RagioneSociale { get; set; }
        public string Email { get; set; }
        public string NumTelefono { get; set; }
        public string Descrizione { get; set; }
        public int IdTipologia { get; set; }
        public string Indirizzo { get; set; }
        public string Citta { get; set; }

        public string ZipOrPostalCode { get; set; }
        public string Country { get; set; }
        public double? Latitudine { get; set; }
        public double? Longitudine { get; set; }
        public DateTime DataCreazione { get; set; }
        public string IdUserOwner { get; set; }
        public string SecurityToken { get; set; }
        public DateTime? DataProvisioning { get; set; }
        public string UrlRoute { get; set; }
        public string OrarioApertura { get; set; }
        public string StorageContainer { get; set; }

    }
}
