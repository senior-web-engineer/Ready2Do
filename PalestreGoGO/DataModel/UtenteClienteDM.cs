﻿using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public class UtenteClienteDM
    {
        public int IdCliente { get; set; }

        public UserReferenceDM UserRef { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string DisplayName { get; set; }
        public DateTime? UtlimoAggiornamento { get; set; }
        public DateTime? DataAssociazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public ClienteUtenteStato Stato { get; set; }

        public UtenteClienteDM()
        {
            Stato = ClienteUtenteStato.Unknown;
        }
    }
}