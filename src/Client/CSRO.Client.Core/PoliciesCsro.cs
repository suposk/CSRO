using System.Collections.Generic;
using System.Security.Claims;

namespace CSRO.Client.Core
{
    public static class PoliciesCsro
    {
        public const string CanApproveAdoRequest = "CanApproveAdoRequest-Policy";

        /// <summary>
        /// Define all Policies and claims
        /// </summary>
        public static Dictionary<string, Claim> PolicyClaimsDictionary = new Dictionary<string, Claim>() 
        {
            { PoliciesCsro.CanApproveAdoRequest,  new Claim(ClaimTypesCsro.CanApproveAdoRequest, true.ToString())},
        };
    }
}
