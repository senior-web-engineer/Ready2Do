using FluentValidation;
using ready2do.model.common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PalestreGoGo.WebAPI.FluentValidators
{
    public class ImmagineClienteValidator:AbstractValidator<ImmagineClienteInputDM>
    {
        public ImmagineClienteValidator()
        {
            RuleFor(i => i.IdCliente).GreaterThan(0);
            RuleFor(i => i.IdTipoImmagine).NotNull();
            RuleFor(i => i.Nome).NotEmpty();
        }
    }
}
