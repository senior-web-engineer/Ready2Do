using System;

namespace ready2do.model.common
{
    public class UtenteClienteDetailsDM: UtenteClienteDM
    {
        [Obsolete("Rifattorizzare modellando meglio lo stato, separandole inproprietà diverse")]
        public ClienteUtenteStato Stato { get; set; }

        public UtenteClienteDetailsDM()
        {
            Stato = ClienteUtenteStato.Unknown;
        }
    }
}
