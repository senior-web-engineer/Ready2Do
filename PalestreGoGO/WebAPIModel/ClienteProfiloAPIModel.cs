using Newtonsoft.Json;
using ready2do.model.common;

namespace PalestreGoGo.WebAPIModel
{
    /// <summary>
    /// Dettagli di un Cliente
    /// </summary>
    public class ClienteProfiloAPIModel
    {
        public ClienteAnagraficaDM Anagrafica { get; set; }

        public ImmagineClienteInputDM ImmagineHome { get; set; }

        [JsonProperty("orarioApertura")]
        public OrarioAperturaDM OrarioApertura { get; set; }
    }

}
