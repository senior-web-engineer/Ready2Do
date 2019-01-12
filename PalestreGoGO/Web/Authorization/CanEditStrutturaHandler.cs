using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Serilog;
using System.Threading.Tasks;
using Web.Services;
using Web.Utils;

namespace Web.Authorization
{
    public class CanEditStrutturaHandler : AuthorizationHandler<CadEditStrutturaRequirement>
    {
        private readonly ClienteResolverServices _clienteResolver;

        public CanEditStrutturaHandler(ClienteResolverServices clienteResolver) : base()
        {
            _clienteResolver = clienteResolver;
        }

        protected async override Task HandleRequirementAsync(AuthorizationHandlerContext context, CadEditStrutturaRequirement requirement)
        {
#if DEBUG
            Log.Information("Inizio validazione requirement CadEditStrutturaRequirement");
#endif
            //Nei claim ci portiamo gli ID dei client (per ridurne la size) ma nel contesto al più abbiamo la urlRoute
            AuthorizationFilterContext mvcContext = context.Resource as AuthorizationFilterContext;
            if (context.Resource == null) { return; }
            object urlRoute;
            if (mvcContext.RouteData.Values.TryGetValue("Cliente", out urlRoute))
            {
                int idCliente = await _clienteResolver.GetIdClienteFromRouteAsync(urlRoute as string);
                if (context.User.GetUserTypeForCliente(idCliente).IsAtLeastAdmin()) { context.Succeed(requirement); }
#if DEBUG
                else
                {
                    Log.Warning($"Failed to validate Requirement CadEditStrutturaRequirement for route: {urlRoute}");
                }
#endif
            }
        }
    }
}
