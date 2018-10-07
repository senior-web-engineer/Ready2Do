﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class ScheduleViewModel
    {
        public int? Id { get; set; }
        public int IdCliente { get; set; }
        public string Title { get; set; }
        public int IdTipoLezione { get; set; }
        public int IdLocation { get; set; }
        public DateTime Data { get; set; }
        public TimeSpan OraInizio { get; set; }
        public string Istruttore { get; set; }
        public int PostiDisponibili { get; set; }
        public DateTime CancellabileFinoAl { get; set; }
        public string Note { get; set; }
    }
}