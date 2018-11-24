﻿using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace PalestreGoGo.DataModel
{
    public partial class ScheduleDM
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string Title { get; set; }
        public int IdTipoLezione { get; set; }
        public int IdLocation { get; set; }
        public DateTime DataOraInizio { get; set; }
        public string Istruttore { get; set; }
        public int PostiDisponibili { get; set; }
        public int? PostiResidui { get; set; }
        public DateTime CancellabileFinoAl { get; set; }
        public DateTime DataChiusuraIscrizione { get; set; }
        public DateTime? DataCancellazione { get; set; }
        public string UserIdOwner { get; set; }
        public string Note { get; set; }


        public TipologiaLezioneDM TipologiaLezione { get; set; }
        public LocationDM Location { get; set; }
    }
}