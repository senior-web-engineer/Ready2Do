using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.WebAPIModel
{
    public class AppuntamentoUserApiModel
    {
        public int IdAppuntamento { get; set; }

        public int IdEvento { get; set; }

        public string Nome { get; set; }

        public int IdCliente { get; set; }

        public string NomeCliente { get; set; }

        public DateTime DataOra { get; set; }

        public DateTime DataOraIscrizione { get; set; }

        public DateTime? DataOraCancellazione { get; set; }
    }
}
