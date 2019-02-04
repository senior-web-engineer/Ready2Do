using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.FluentValidators
{
    public class ClientRegistrationAccountInputModelValidator :AbstractValidator<ClientRegistrationAccountInputModel>
    {
        public ClientRegistrationAccountInputModelValidator()
        {
            RuleFor(c => c.Cognome).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Nome).NotEmpty().MaximumLength(100);
            RuleFor(c => c.DisplayName).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Email).NotEmpty().EmailAddress().MaximumLength(256);
            RuleFor(c => c.DisplayName).MaximumLength(20);
            RuleFor(c => c.Password).NotEmpty().MinimumLength(6);
            RuleFor(c => c.PasswordConfirm).NotEmpty().MinimumLength(6);
            RuleFor(c => c.PasswordConfirm).Equal(r => r.Password).WithMessage("Le Password non coincidono");
        }
    }
}
