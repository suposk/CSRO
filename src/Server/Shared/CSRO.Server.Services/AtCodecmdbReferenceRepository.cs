using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace CSRO.Server.Services
{
    public class AtCodecmdbReferenceRepository : IAtCodecmdbReferenceRepository
    {
        public CustomersDbContext Context { get; }

        public AtCodecmdbReferenceRepository(CustomersDbContext context, IApiIdentity apiIdentity)
        {
            Context = context;
        }        

        public Task<List<ResourceSWI>> GetList()
        {
            //return GetList();            
            //var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
            return Context.ResourceSWIs.ToListAsync();
        }
    }
}
