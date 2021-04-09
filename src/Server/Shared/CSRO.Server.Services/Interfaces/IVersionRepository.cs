using CSRO.Server.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Services
{
    public interface IVersionRepository
    {
        Task<AppVersion> GetVersion(string version);
    }
}
