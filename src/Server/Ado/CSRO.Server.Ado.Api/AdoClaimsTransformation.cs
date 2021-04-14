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
        private readonly IRestUserService _restUserService;

        public AdoClaimsTransformation(IRestUserService restUserService)
        {
            _restUserService = restUserService;
        }

        //private readonly ILocalUserService _localUserService;
        //public AdoClaimsTransformation(ILocalUserService localUserService)
        //{
        //    _localUserService = localUserService;
        //}

        public async Task<ClaimsPrincipal> TransformAsync(ClaimsPrincipal principal)
        {
            if (principal.Identity.IsAuthenticated)
            {
                //var aditionalClaims = await _localUserService.GetClaimsByUserNameAsync(principal.Identity.Name);
                var aditionalClaims = await _restUserService.GetClaimsByUserNameAsync(principal.Identity.Name.Replace("live.com#", ""), principal);
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
