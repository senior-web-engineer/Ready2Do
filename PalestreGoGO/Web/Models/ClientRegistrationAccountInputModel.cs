using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Web.Utils;

namespace Web.Models
{
    public class ClientRegistrationAccountInputModel
    {
        public string Nome { get; set; }
        public string Cognome { get; set; }
        public string DisplayName { get; set; }
        public string MobileNumber { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public string PasswordConfirm { get; set; }   
    }
}
