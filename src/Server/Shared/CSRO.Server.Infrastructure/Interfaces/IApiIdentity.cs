using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure
{
    public interface IApiIdentity
    {
        //int GetUserId();
        string GetUserName();
        bool IsAuthenticated();
    }
}
