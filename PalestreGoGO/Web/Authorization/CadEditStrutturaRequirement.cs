using Microsoft.AspNetCore.Authorization;

namespace Web.Authorization
{
    /// <summary>
    /// Requirement per le operazioni di modifica su una struttura
    /// </summary>
    public class CadEditStrutturaRequirement : IAuthorizationRequirement
    {
        //Nothing here
        public CadEditStrutturaRequirement()
        {

        }
    }
}
