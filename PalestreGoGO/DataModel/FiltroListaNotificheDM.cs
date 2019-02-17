using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    [JsonConverter(typeof(StringEnumConverter))]
    public enum FiltroListaNotificheDM: byte
    {
        Tutte = 0,
        SoloAttive = 1,
        SoloNonLette =  2
    }
}
