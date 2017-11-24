using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel
{
    public class Coordinate
    {
        public Coordinate()
        {

        }

        public Coordinate( float latitudine, float longitudine)
        {
            this.Latitudine = latitudine;
            this.Longitudine = longitudine;
        }

        [JsonProperty("lat")]
        [Required]
        public float Latitudine { get; set; }

        [JsonProperty("long")]
        [Required]
        public float Longitudine { get; set; }
    }
}
