using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Palestregogo.STS.Model.Identity
{
    public class AppUser : IdentityUser<Guid>
    {
        public AppUser()
        {

        }

        public AppUser(string email, string nome, string cognome, string telefono, string token)
        {
            UserName = email;
            Email = email;
            FirstName = nome;
            LastName = cognome;
            PhoneNumber = telefono;
            CreationToken = token;
        }

        public string FirstName { get; set; }
        public string LastName { get; set; }
        
        public string CreationToken { get; set; }
        //public string Description { get; set; }
    }
}
