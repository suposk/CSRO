using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IVersionService
    {
        Task<AppVersion> AddVersion(AppVersion add);
        Task<bool> DeleteVersion(int id);
        Task<List<AppVersion>> GetAllVersion();
        Task<AppVersion> GetVersion(string version = "0");
    }
}
