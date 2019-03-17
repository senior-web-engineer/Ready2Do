using FluentValidation;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Web.Models.FluentValidators
{
    public class ImmagineClienteValidator: AbstractValidator<ImmagineClienteDM>
    {
        public ImmagineClienteValidator()
        {
            RuleFor(i => i.IdCliente).NotNull().GreaterThan(0);
            RuleFor(i => i.IdTipoImmagine).NotNull();
            RuleFor(i => i.Nome).NotEmpty();
            RuleFor(i => i.Ordinamento).GreaterThanOrEqualTo(0);
            RuleFor(i => i.Url).NotEmpty();
        }
    }
}
