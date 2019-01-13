using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class UserProfileViewModel : UtenteDM
    {
        //Solo lettura
        public bool EMailConfirmed { get; set; }
        //Solo lettura
        public bool TelephoneConfirmed { get; set; }

        public static UserProfileViewModel FromUtenteDM(UtenteInputDM input)
        {
            return new UserProfileViewModel()
            {
                Nome = input.Nome,
                Cognome = input.Cognome,
                DisplayName = input.DisplayName,
                TelephoneNumber = input.TelephoneNumber
            };
        }

        public static UserProfileViewModel FromUtenteDM(UtenteDM input)
        {
            var result = FromUtenteDM((UtenteInputDM)input);
            result.UserId = input.UserId;
            result.Email = input.Email;
            return result;
        }
    }

}
