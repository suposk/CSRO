using CSRO.Server.Infrastructure;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Services
{
    public class ApiIdentity : IApiIdentity
    {
        private readonly IHttpContextAccessor _context;
        public ApiIdentity(IHttpContextAccessor context)
        {
            _context = context;
        }
        //public int GetUserId()
        //{
        //    return Convert.ToInt16(_context.HttpContext.User.FindFirstValue(ClaimTypes.NameIdentifier));
        //}
        public string GetUserName()
        {
            return _context.HttpContext?.User?.Identity?.Name;
        }

    }
}
