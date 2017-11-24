using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class ClientiMetadati 
    {        
        public int IdCliente { get; set; }
        [StringLength(100)]
        public string Key { get; set; }
        [Required]
        public string Value { get; set; }

        [ForeignKey("IdCliente")]
        [InverseProperty("ClientiMetadati")]
        public Clienti IdClienteNavigation { get; set; }
    }
}
