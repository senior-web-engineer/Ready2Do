using FluentValidation;
using PalestreGoGo.WebAPIModel;

namespace PalestreGoGo.WebAPI.ViewModel.Validators
{
    public class NuovoClienteValidator : AbstractValidator<NuovoClienteViewModel>
    {
        public NuovoClienteValidator()
        {
            RuleFor(c => c.Nome).NotEmpty().MaximumLength(100);
            RuleFor(c => c.RagioneSociale).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Email).NotEmpty().MaximumLength(100).EmailAddress();
            RuleFor(c => c.NumTelefono).NotEmpty().MaximumLength(50);
            RuleFor(c => c.Indirizzo).NotEmpty().MaximumLength(250);
            RuleFor(c => c.Citta).NotEmpty().MaximumLength(100);
            RuleFor(c => c.ZipOrPostalCode).NotEmpty().MaximumLength(10);
            RuleFor(c => c.Country).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Coordinate).NotNull();
            RuleFor(c => c.NuovoUtente).NotNull();
        }
    }
}
