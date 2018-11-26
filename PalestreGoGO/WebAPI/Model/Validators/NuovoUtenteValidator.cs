using FluentValidation;
using PalestreGoGo.WebAPIModel;

namespace PalestreGoGo.WebAPI.ViewModel.Validators
{
    public class NuovoUtenteValidator : AbstractValidator<NuovoUtenteViewModel>
    {
        public NuovoUtenteValidator()
        {
            RuleFor(u => u.Nome).NotEmpty().MaximumLength(100);
            RuleFor(u => u.Cognome).NotEmpty().MaximumLength(100);
            RuleFor(u => u.Email).NotEmpty().MaximumLength(256).EmailAddress();
            RuleFor(u => u.Telefono).NotEmpty().MaximumLength(50);
            RuleFor(u => u.Password).NotEmpty().MaximumLength(200);            
        }
    }
}
