using System;
using System.Collections.Generic;

namespace ready2do.model.common
{
    public class AssociazioneUtenteClienteDM
    {
        public UtenteClienteDM UtenteCliente { get; set; }

        public int IdCliente { get; set; }

        //public DateTime? DataAssociazione { get; set; }

        ///// <summary>
        ///// Questa proprietà è valorizzata solo se l'ultima associazione è stata cancellata.
        ///// Se l'associazione è stata cancellata e poi ricreata la proprietà sarà NULL
        ///// </summary>
        //public DateTime? DataCancellazioneAssociazione { get; set; }

        //Lista abbonamenti presso il Cliente
        public IEnumerable<AbbonamentoUtenteDM> Abbonamenti { get; set; }

        //Lista degli appuntamenti presso il Cliente
        public IEnumerable<AppuntamentoDM> Appuntamenti { get; set; }

        public IEnumerable<AppuntamentoDaConfermareDM> AppuntamentiDaConfermare { get; set; }

        public IEnumerable<UtenteClienteCertificatoDM> Certificati { get; set; }
    }
}
