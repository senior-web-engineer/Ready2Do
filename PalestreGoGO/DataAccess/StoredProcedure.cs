using System;
using System.Collections.Generic;
using System.Text;

namespace PalestreGoGo.DataAccess
{
    public sealed class StoredProcedure
    {
        public const string SP_RICHIESTE_REGISTRAZIONE_INSERT = "RichiestaRegistrazione_Insert";
        public const string SP_RICHIESTE_REGISTRAZIONE_COMPLETA = "RichiestaRegistrazione_Completa";
        
        public const string SP_USER_CLIENTI_FOLLOWED = "Utenti_ClientiFollowed";
        public const string SP_USER_FOLLOW_CLIENTE = "Utenti_ClienteIsFollowed";
    }
}
