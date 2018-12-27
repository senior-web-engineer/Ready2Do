using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class ClienteAnagraficaDM
    {
        public int? Id { get; set; }
        public string Nome { get; set; }
        [JsonProperty("ragSociale")]
        public string RagioneSociale { get; set; }
        public string Email { get; set; }
        public string NumTelefono { get; set; }
        public string Descrizione { get; set; }
        [JsonProperty("tipologia")]
        public string Indirizzo { get; set; }
        public string Citta { get; set; }
        [JsonProperty("cap")]
        public string ZipOrPostalCode { get; set; }
        public string Country { get; set; }
        public double? Latitudine { get; set; }
        public double? Longitudine { get; set; }
        public string UrlRoute { get; set; }
    }

    public class NuovoClienteInputDM : ClienteAnagraficaDM
    {
        public string StorageContainer { get; set; }
        public string CorrelationId { get; set; }
        public int IdTipologia { get; set; }
        public string IdUserOwner { get; set; }
    }
    public class ClienteDM : ClienteAnagraficaDM
    {
        public DateTime? DataCreazione { get; set; }
        public DateTime? DataProvisioning { get; set; }
        public TipologiaClienteDM TipoCliente {get;set;}
        public string StorageContainer { get; set; }
        public string OrarioApertura { get; set; }

    }
}
