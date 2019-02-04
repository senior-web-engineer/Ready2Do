using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models
{
    public class ClientRegistrationViewModel: ClientRegistrationStrutturaInputModel
    {

        public IEnumerable<SelectListItem> TipologieClienti { get; set; }

    }
}
