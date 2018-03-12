﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class CoordinateViewModel
    {
        public CoordinateViewModel()
        {

        }
         public CoordinateViewModel( float latitudine, float longitudine)
        {
            Latitudine = latitudine;
            Longitudine = longitudine;
        }

        [JsonProperty("lat")]
        [Required]
        public float Latitudine { get; set; }

        [JsonProperty("long")]
        [Required]
        public float Longitudine { get; set; }
    }
}
