using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class CoordinateAPIModel
    {
        public CoordinateAPIModel()
        {

        }
         public CoordinateAPIModel( float latitudine, float longitudine)
        {
            Latitudine = latitudine;
            Longitudine = longitudine;
        }

        [JsonProperty("lat")]
        public float Latitudine { get; set; }

        [JsonProperty("long")]
        public float Longitudine { get; set; }
    }
}
