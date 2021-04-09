using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure
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
            if (_context?.HttpContext == null)
                return null;

            var defname = _context.HttpContext?.User?.Identity?.Name;
            if (!string.IsNullOrWhiteSpace(defname))
                return defname;

            List<string> list = new List<string>();
            //var nameId = _context.HttpContext?.User.FindFirst(c => c.Type == ClaimTypes.NameIdentifier);
            var email = _context.HttpContext?.User.FindFirst(c => c.Type == ClaimTypes.Email);
            //var upn = _context.HttpContext?.User.FindFirst(c => c.Type == ClaimTypes.WindowsAccountName);
            var userData = _context.HttpContext?.User.FindFirst(c => c.Type == ClaimTypes.UserData);

            //foreach(var item in _context.HttpContext?.User?.Claims)
            //{
            //    if (item.Type.ToLower() == "name")
            //        list.Add(item.Value);
            //}
            if (list.Count == 0 && email != null && !string.IsNullOrWhiteSpace(email.Value))
                return email.Value;
            if (list.Count > 0)
            {
                var shortestName = list.OrderBy(name => name.Length).FirstOrDefault();
                return shortestName;
            }
            return _context.HttpContext?.User?.Identity?.Name;
        }

    }
}
