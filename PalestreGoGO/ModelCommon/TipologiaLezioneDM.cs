﻿using System;

namespace ready2do.model.common
{
    public partial class TipologiaLezioneDM
    {
        public int Id { get; set; }
        public int IdCliente { get; set; }
        public string Nome { get; set; }
        public string Descrizione { get; set; }
        public int Durata { get; set; }
        public int? MaxPartecipanti { get; set; }
        public short? LimiteCancellazioneMinuti { get; set; }
        public short Livello { get; set; }
        public DateTime DataCreazione { get; set; }
        public DateTime? DataCancellazione { get; set; }
    }
}