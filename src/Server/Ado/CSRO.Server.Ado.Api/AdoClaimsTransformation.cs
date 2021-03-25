﻿using CSRO.Server.Services;
using Microsoft.AspNetCore.Authentication;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api
{
    public class AdoClaimsTransformation : IClaimsTransformation
    {
        private readonly ILocalUserService _localUserService;

        public AdoClaimsTransformation(ILocalUserService localUserService)
        {
            _localUserService = localUserService;
        }

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                var aditionalClaims = await _localUserService.GetUserClaimsByUserNameAsync(principal.Identity.Name);
                if (aditionalClaims.HasAnyInCollection())
                {
                    var identity = principal.Identity as ClaimsIdentity;
                    if (identity == null)
                    {
                        //throw ex
                        return principal;
                    }

                    //
                    foreach(var cl in aditionalClaims)
                    {
                        var claim = new Claim(cl.Type, cl.Value);
                        if (!principal.Claims.Contains(claim))
                            identity.AddClaim(claim);
                    }
                }
            }
            return principal;
        }
    }
}
