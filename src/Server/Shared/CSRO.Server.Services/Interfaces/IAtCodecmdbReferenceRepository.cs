using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CSRO.Server.Services
{
    public interface IAtCodecmdbReferenceRepository
    {
        Task<List<ResourceSWI>> GetList();
    }
}
