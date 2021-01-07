using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CSRO.Server.Services
{
    public interface IVmRepository : IRepository<Vm>
    {

    }

    public class VmRepository : Repository<Vm>, IVmRepository
    {
        private readonly IRepository<Vm> _repository;
        private AppVersionContext _context;

        public VmRepository(IRepository<Vm> repository, AppVersionContext context, IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _repository = repository;
            _context = context;
        }

        public override Task<List<Vm>> GetList()
        {
            //return base.GetList();
            //var exist = await _repository.GetFilter(a => a.VersionFull == version);
            //var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
            return _repository.GetListFilter(a => a.IsDeleted != true);
        }
    }
}
