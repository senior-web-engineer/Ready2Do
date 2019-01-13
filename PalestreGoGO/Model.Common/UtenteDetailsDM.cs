using System;
using System.Collections.Generic;
using System.Text;

namespace ready2do.model.common
{

    /// <summary>
    /// Rappresenta i dettagli di un utente
    /// </summary>
    public class UtenteDetailsDM: UtenteDM
{
        //Nota: al momento non include i dati del profilo

        public string UserId { get; set; }

        public IList<ClienteAssociatoUtenteDM> ClientiAssociati { get; set; }

        public UtenteDetailsDM()
        {

        }

        public UtenteDetailsDM(string userId)
        {
            this.UserId = userId;
        }
    }
}
