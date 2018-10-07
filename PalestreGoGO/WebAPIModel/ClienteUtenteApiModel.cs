﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class ClienteUtenteApiModel
    {
        public int IdCliente { get; set; }
        public Guid IdUtente { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string Email { get; set; }

        public DateTime DataAssociazione { get; set; }

    }
}