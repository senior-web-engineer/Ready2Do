using Newtonsoft.Json;
using PalestreGoGo.WebAPIModel;

namespace PalestreGoGo.WebAPI.ViewModel
{
    public class ClienteBase
    {
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("ragSociale")]
        public string RagioneSociale { get; set; }

        [JsonProperty("email")]
        public string Email { get; set; }

        [JsonProperty("phone")]
        public string NumTelefono { get; set; }

        [JsonProperty("tipologia")]
        public short IdTipologia { get; set; }

        [JsonProperty("indirizzo")]
        public string Indirizzo { get; set; }

        [JsonProperty("citta")]
        public string Citta { get; set; }

        [JsonProperty("cap")]
        public string ZipOrPostalCode { get; set; }

        [JsonProperty("country")]
        public string Country { get; set; }

        //[Required]
        [JsonProperty("coordinate")]
        public CoordinateViewModel Coordinate { get; set; }

    }
}
