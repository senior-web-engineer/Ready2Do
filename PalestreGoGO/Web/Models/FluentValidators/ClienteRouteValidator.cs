using FluentValidation.Validators;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Web.Proxies;
using Web.Utils;

namespace Web.Models.FluentValidators
{
    /// <summary>
    /// Custom Fluent Validator per supportare la verifica lato SERVER dell'UrlRoute di un cliente.
    /// Alternativa al RemoteAttribute della validazione basata su attibuti
    /// </summary>
    public class ClienteRouteValidator : PropertyValidator
    {
        private readonly ClienteProxy _apiClient;

        public ClienteRouteValidator(ClienteProxy apiClient) : base("L'indirizzo specificato risulta già utilizzato.")
        {
            _apiClient = apiClient;
        }

        protected override Task<bool> IsValidAsync(PropertyValidatorContext context, CancellationToken cancellation)
        {
            return _apiClient.CheckUrlRoute(context.PropertyValue as string);
        }

        protected override bool IsValid(PropertyValidatorContext context)
        {
            return IsValidAsync(context, CancellationToken.None).Result;
        }
    }

}
