using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace PalestreGoGo.WebAPIModel
{
    public class NuovoClienteViewModel
    {
        public NuovoClienteViewModel()
        {
            this.Coordinate = new CoordinateViewModel();
            this.NuovoUtente = new NuovoUtenteViewModel();
        }

        //######### DATI ANAGRAFICI CLIENTE // 
        //[Required]
        //[MaxLength(100)]
        [JsonProperty("nome")]
        public string Nome { get; set; }

        [JsonProperty("cognome")]
        public string Cognome { get; set; }

        //[Required]
        //[MaxLength(100)]
        [JsonProperty("ragSociale")]
        public string RagioneSociale { get; set; }

        //[Required]
        //[MaxLength(100)]
        //[EmailAddress]
        [JsonProperty("email")]
        public string Email { get; set; }

        //[Required]
        //[MaxLength(50)]
        //[Phone]
        [JsonProperty("phone")]
        public string NumTelefono { get; set; }

        //[Required]
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

        //[Required]
        [JsonProperty("userInfo")]
        public NuovoUtenteViewModel NuovoUtente { get; set; }
    }
}
