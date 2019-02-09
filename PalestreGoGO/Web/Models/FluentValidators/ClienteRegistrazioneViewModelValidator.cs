using FluentValidation;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Web.Proxies;
using Web.Utils;

namespace Web.Models.FluentValidators
{
    public class ClienteRegistrazioneViewModelValidator : AbstractValidator<ClienteRegistrazioneViewModel>
    {
        const string URL_SEGMENT_CHARS = @"^[0-9a-zA-Z\-_]+$";
        private readonly ClienteProxy _apiClient;

        public ClienteRegistrazioneViewModelValidator(ClienteProxy apiClient)
        {
            _apiClient = apiClient;
            RuleFor(c => c.URL).NotEmpty().MinimumLength(3).Matches(URL_SEGMENT_CHARS);
            RuleFor(c => c.URL).SetValidator(new ClienteRouteValidator(_apiClient));

            RuleFor(c => c.NomeStruttura).NotEmpty().MaximumLength(100);
            RuleFor(c => c.RagioneSociale).NotEmpty().MaximumLength(100);
            RuleFor(c => c.EmailStruttura).NotEmpty().MaximumLength(256).EmailAddress();
            RuleFor(c => c.Telefono).MaximumLength(50);
            RuleFor(c => c.Indirizzo).NotEmpty().MaximumLength(255)
                    .Must((c, indirizzo) => { return c.EsitoLookup == 1; })
                        .WithMessage("E' necessario selezionare un indirizzo tra quelli proposti");
            RuleFor(c => c.Indirizzo).Must((c, indirizzo) => { return ValidaCoordinate(c.Coordinate); })
                        .WithMessage("E' necessario selezionare un indirizzo tra quelli proposti");


        }

        private bool ValidaCoordinate(string coordinate)
        {
            if (string.IsNullOrWhiteSpace(coordinate)) return false;
            var match = Constants.COORDINATE_REGEX.Match(coordinate);
            if (!match.Success || (match.Groups.Count != 3)) return false;
            if (!float.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var _)) return false;
            if (!float.TryParse(match.Groups[2].Value, System.Globalization.NumberStyles.Float, CultureInfo.InvariantCulture, out var _)) return false;
            return true;
        }
    }
}
