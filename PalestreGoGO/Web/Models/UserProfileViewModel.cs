﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class UserProfileViewModel
    {
        public IEnumerable<AppuntamentoUtenteViewModel> Appuntamenti { get; set; }

        public UserProfileViewModel()
        {

        }
    }

}
