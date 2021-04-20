using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using CSRO.Server.Entities;

namespace CSRO.Server.Services
{
    public interface IAtCodecmdbReferenceRepository
    {
        CustomersDbContext Context { get; }

        Task<List<ResourceSWI>> GetList();
    }
}
