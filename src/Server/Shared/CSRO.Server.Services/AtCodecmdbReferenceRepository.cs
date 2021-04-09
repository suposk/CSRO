using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CSRO.Server.Services
{
    public class AtCodecmdbReferenceRepository : Repository<AtCodecmdbReference>, IAtCodecmdbReferenceRepository
    {
        private readonly IRepository<AtCodecmdbReference> _repository;
        private BillingContext _context;

        public AtCodecmdbReferenceRepository(IRepository<AtCodecmdbReference> repository, BillingContext context, IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _repository = repository;
            _context = context;
        }

        public override Task<List<AtCodecmdbReference>> GetList()
        {
            return GetList();            
            //var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
            //return _repository.GetListFilter(a => a.IsDeleted != true);
        }
    }
}
