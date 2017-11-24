using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PalestreGoGo.WebAPI.ViewModel
{
    public class NuovoClienteViewModel
    {
        public NuovoClienteViewModel()
        {
            this.Coordinate = new Coordinate();
            this.NuovoUtente = new NuovoUtenteViewModel();
        }

        //######### DATI ANAGRAFICI CLIENTE // 
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

        [JsonProperty("coordinate")]
        public Coordinate Coordinate { get; set; }

        [JsonProperty("userInfo")]
        public NuovoUtenteViewModel NuovoUtente { get; set; }
    }
}
