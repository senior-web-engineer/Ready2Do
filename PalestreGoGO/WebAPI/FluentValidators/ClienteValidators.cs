using FluentValidation;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.FluentValidators
{
    public class ClienteBaseValidator<T> : AbstractValidator<T> where T: ClienteAnagraficaDM
    {
        public ClienteBaseValidator()
        {
            RuleFor(c => c.Nome).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Indirizzo).NotEmpty().MaximumLength(250);
            RuleFor(c => c.Latitudine).NotNull();
            RuleFor(c => c.Longitudine).NotNull();
            RuleFor(c => c.RagioneSociale).NotEmpty().MaximumLength(100);
            RuleFor(c => c.Email).NotEmpty().EmailAddress();
            RuleFor(c => c.NumTelefono).MaximumLength(50);
            RuleFor(c => c.Descrizione).MaximumLength(1000);
            RuleFor(c => c.Citta).NotEmpty().MaximumLength(100);
            RuleFor(c => c.ZipOrPostalCode).MaximumLength(10);
            RuleFor(c => c.Country).MaximumLength(100);     
        }
    }

    public class NuovoClienteInputDMValidator: ClienteBaseValidator<NuovoClienteInputDM>
    {
        public NuovoClienteInputDMValidator()
        {
            RuleFor(c => c.IdTipologia).GreaterThan(0);
            RuleFor(c => c.Id).Null();
            RuleFor(c => c.IdUserOwner).Null();
            RuleFor(c => c.StorageContainer).Null();
        }
    }
}
