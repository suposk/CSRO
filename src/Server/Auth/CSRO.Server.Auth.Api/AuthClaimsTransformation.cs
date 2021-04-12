using CSRO.Server.Services;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CSRO.Server.Auth.Api
{
    public class AuthClaimsTransformation : IClaimsTransformation
    {
        private readonly ILocalUserService _localUserService;

        public AuthClaimsTransformation(ILocalUserService localUserService)
        {
            _localUserService = localUserService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                var aditionalClaims = await _localUserService.GetClaimsByUserNameAsync(principal.Identity.Name);
                if (aditionalClaims.HasAnyInCollection())
                {
                    var identity = principal.Identity as ClaimsIdentity;
                    if (identity == null)
                    {
                        //throw ex
                        return principal;
                    }
                    identity.AddClaims(aditionalClaims);
                }
            }
            return principal;
        }
    }
}
