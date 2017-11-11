using IdentityServer4.Models;
using IdentityServer4.Services;
using IdentityServer4.Extensions;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using Palestregogo.STS.Model.Identity;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Security.Claims;
using IdentityModel;
using IdentityServer4;

namespace Palestregogo.STS.Business
{
    public class STSProfileService: IProfileService
    {
        private readonly IUserClaimsPrincipalFactory<AppUser> _claimsFactory;
        private readonly UserManager<AppUser> _userManager;
        private readonly ILogger _logger;

        public STSProfileService(ILogger logger, UserManager<AppUser> userManager, IUserClaimsPrincipalFactory<AppUser> claimsFactory)
        {
            _logger = logger;
            _userManager = userManager;
            _claimsFactory = claimsFactory;
        }

        public async Task GetProfileDataAsync(ProfileDataRequestContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            //Crea un ClaimsPrincipal a partire da uno AppUser ==> popola tutti i claim (UserClaims + RoleClaims + Roles?)
            var principal = await _claimsFactory.CreateAsync(user);

            var claims = principal.Claims.ToList();
            claims = claims.Where(claim => context.RequestedClaimTypes.Contains(claim.Type)).ToList();

            /*RIVEDERE*/
            claims.Add(new Claim(JwtClaimTypes.GivenName, user.UserName));
            claims.Add(new Claim(IdentityServerConstants.StandardScopes.Email, user.Email));

            context.IssuedClaims = claims;
        }

        public async Task IsActiveAsync(IsActiveContext context)
        {
            var sub = context.Subject.GetSubjectId();
            var user = await _userManager.FindByIdAsync(sub);
            /*VERIFICARE!*/
            //Se non troviamo l'utente oppure è LockedOut ==> Not Active
            context.IsActive = !((user == null) || ((user.LockoutEnabled) && (user.LockoutEnd > DateTimeOffset.Now)));
        }
    }
}
