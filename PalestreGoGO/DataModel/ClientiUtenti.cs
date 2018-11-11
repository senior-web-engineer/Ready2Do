﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class ClientiUtenti
    {
        public int IdCliente { get; set; }

        public string IdUtente { get; set; }

        public DateTime DataCreazione { get; set; }

        [ForeignKey("IdCliente")]
        [InverseProperty("ClientiUtenti")]
        public Clienti Cliente { get; set; }
    }
}
