using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataModel
{
    public partial class ClienteFollowed
    {
        public int IdCliente { get; set; }

        public string Nome { get; set; }

        public string RagioneSociale { get; set; }

        public DateTime DataFollowing { get; set; }

        public bool AbbonamentoValido { get; set; }
    }
}
