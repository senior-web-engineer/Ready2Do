using FluentValidation;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Web.Proxies;
using Web.Utils;

namespace Web.Models.FluentValidators
{
    public class ClienteRegistrazioneInputModelValidator: AbstractValidator<ClienteRegistrazioneInputModel>
    {
        const string URL_SEGMENT_CHARS = @"^[0-9a-zA-Z\-_]+$";
        private readonly ClienteProxy _apiClient;

        public ClienteRegistrazioneInputModelValidator(ClienteProxy apiClient)
        {
            _apiClient = apiClient;
            RuleFor(c => c.URL).NotEmpty().MinimumLength(3).Matches(URL_SEGMENT_CHARS);
            RuleFor(c => c.URL).SetValidator(new ClienteRouteValidator(_apiClient));

            RuleFor(c => c.NomeStruttura).NotEmpty().MaximumLength(100);
            RuleFor(c => c.RagioneSociale).NotEmpty().MaximumLength(100);
            RuleFor(c => c.EmailStruttura).NotEmpty().MaximumLength(256).EmailAddress();
            RuleFor(c => c.Indirizzo).NotEmpty().MaximumLength(255)
                    .Must((c, indirizzo) => { return c.EsitoLookup == 1; })
                        .WithMessage("E' necessario selezionare un indirizzo tra quelli proposti");


        }
    }
}
