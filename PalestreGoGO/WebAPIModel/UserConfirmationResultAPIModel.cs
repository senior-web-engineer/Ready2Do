using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPIModel
{
    public class UserConfirmationResultAPIModel
    {
        public UserConfirmationResultAPIModel() :this(false)
        {
        }

        public UserConfirmationResultAPIModel(bool esito = false)
        {
            Esito = esito;
        }

        public UserConfirmationResultAPIModel(string idUser)
        {
            this.Esito = true;
            IdUser = idUser;
        }
        public bool Esito { get; set; }
        public string IdUser { get; set; }

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
