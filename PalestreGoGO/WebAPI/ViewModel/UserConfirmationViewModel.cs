using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.ViewModel
{
    public class UserConfirmationViewModel
    {
        public UserConfirmationViewModel(bool esito = false)
        {
            Esito = esito;
        }

        public UserConfirmationViewModel(Guid idUserOwner)
        {
            this.Esito = true;
            IdUserOwner = idUserOwner;
        }
        public bool Esito { get; set; }
        public Guid IdUserOwner { get; set; }
    }
}
