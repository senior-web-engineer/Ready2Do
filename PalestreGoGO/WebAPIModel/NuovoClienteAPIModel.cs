using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PalestreGoGo.WebAPIModel
{
    public class NuovoClienteAPIModel
    {
        public NuovoClienteAPIModel()
        {
            this.Coordinate = new CoordinateAPIModel();
            this.NuovoUtente = new NuovoUtenteViewModel();
        }

        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("cognome")]
        public string Cognome { get; set; }

        [JsonProperty("descrizione")]
        public string Descrizione { get; set; }

        [JsonProperty("ragSociale")]
        public string RagioneSociale { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string NumTelefono { get; set; }

        [JsonProperty("tipologia")]
        public int IdTipologia { get; set; }

        [JsonProperty("indirizzo")]
        public string Indirizzo { get; set; }

        [JsonProperty("citta")]
        public string Citta { get; set; }

        [JsonProperty("cap")]
        public string ZipOrPostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        [JsonProperty("coordinate")]
        public CoordinateAPIModel Coordinate { get; set; }

        [JsonProperty("userInfo")]
        public NuovoUtenteViewModel NuovoUtente { get; set; }

        [JsonProperty("urlRoute")]
        public string UrlRoute { get; set; }
    }
}
