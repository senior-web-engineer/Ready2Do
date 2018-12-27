using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{
    public class ClienteFollowedDM
    {
        public int IdCliente { get; set; }

        public string Nome { get; set; }

        public string RagioneSociale { get; set; }

        public DateTime DataFollowing { get; set; }

        public bool AbbonamentoValido { get; set; }
    }
}
