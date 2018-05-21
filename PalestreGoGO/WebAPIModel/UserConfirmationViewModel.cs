using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class UserConfirmationViewModel
    {
        public UserConfirmationViewModel() :this(false)
        {
        }

        public UserConfirmationViewModel(bool esito = false)
        {
            Esito = esito;
        }

        public UserConfirmationViewModel(Guid idUser)
        {
            this.Esito = true;
            IdUser = idUser;
        }
        public bool Esito { get; set; }
        public Guid IdUser { get; set; }

        /// <summary>
        /// Valorizzato SOLO PER GLI UTENTI ORDINARI e solo se in fase di registrazione 
        /// è stato specificato un idref (Id Struttura Affiliata da cui è iniziato ol processo di registrazione)
        /// </summary>
        public int? IdStrutturaAffiliate { get; set; }

        /// <summary>
        /// Valorizzato SOLO PER I CLIENTE con l'id della struttura che è stata registrata contestualmente alla creazione dell'utenza
        /// </summary>
        public int? IdCliente { get; set; }
    }
}
