using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class TipologiaNotificaApiModel
    {
        public int Id { get; set; }
        [MaxLength(50)]
        public string Code { get; set; }
        public bool UserDismissable { get; set; }
        public long? AutoDismisAfter { get; set; }
        public int Priority { get; set; }
    }
}
