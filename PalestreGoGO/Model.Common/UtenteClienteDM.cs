using System;

namespace ready2do.model.common
{
    public class UtenteClienteDM
    {
        public int IdCliente { get; set; }

        public string UserId { get; set; }
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string DisplayName { get; set; }
        public DateTime? UtlimoAggiornamento { get; set; }
        public DateTime? DataAssociazione { get; set; }
        public DateTime? DataCancellazione { get; set; }

        public UtenteClienteDM()
        {
        }
    }
}
