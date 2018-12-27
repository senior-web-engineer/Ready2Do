using FluentValidation;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.FluentValidators
{
    public class UtenteValidator: AbstractValidator<NuovoUtenteViewModel>
    {
        public UtenteValidator()
        {
            RuleFor(u => u.Email).NotEmpty().EmailAddress();
            RuleFor(u => u.Cognome).NotEmpty().MaximumLength(100);
            RuleFor(u => u.Nome).NotEmpty().MaximumLength(100);
            RuleFor(u => u.DisplayName).NotEmpty().MaximumLength(100);
            RuleFor(u => u.Password).NotEmpty().MinimumLength(6);//.Must(BeStrongPwd); //Rinforzare controllo sulla password?

        }

        private bool BeStrongPwd(string pwd)
        {
            return true;
        }
    }
}
