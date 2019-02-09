using FluentValidation;
using PalestreGoGo.WebAPIModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.FluentValidators
{
    public class NuovoClienteAPIModelValidator: AbstractValidator<NuovoClienteAPIModel>
    {
        public NuovoClienteAPIModelValidator()
        {
            RuleFor(c => c.NomeStruttura).NotEmpty().MaximumLength(100);
            RuleFor(c => c.IdTipologia).GreaterThanOrEqualTo(0);
            RuleFor(c => c.Indirizzo).NotEmpty().MaximumLength(250);
            RuleFor(c => c.Coordinate).NotNull();
            RuleFor(c => c.RagioneSociale).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.NumTelefono).MaximumLength(50);
            RuleFor(c => c.Descrizione).MaximumLength(1000);
            RuleFor(c => c.Citta).NotEmpty().MaximumLength(100);
            RuleFor(c => c.ZipOrPostalCode).MaximumLength(10);
            RuleFor(c => c.Country).MaximumLength(100);
        }
    }
}
