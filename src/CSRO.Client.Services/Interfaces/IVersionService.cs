using CSRO.Client.Services.Dtos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IVersionService
    {
        Task<VersionDto> AddVersion(VersionDto add);
        Task<bool> DeleteVersion(int id);
        Task<List<VersionDto>> GetAllVersion();
        Task<VersionDto> GetVersion(string version = "0");
    }
}
