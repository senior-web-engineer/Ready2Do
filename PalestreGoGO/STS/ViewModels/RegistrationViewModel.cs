using Microsoft.AspNetCore.Mvc.Rendering;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Palestregogo.STS.UI.Model
{
    public class RegistrationViewModel: RegistrationInputModel
    {

        public IEnumerable<SelectListItem> TipologieClienti { get; set; }
       
    }
}
