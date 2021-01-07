using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CSRO.Server.Services
{
    public interface IVmTicketRepository : IRepository<VmTicket>
    {

    }

    public class VmTicketRepository : Repository<VmTicket>, IVmTicketRepository
    {
        private readonly IRepository<VmTicket> _repository;
        private AppVersionContext _context;

        public VmTicketRepository(IRepository<VmTicket> repository, AppVersionContext context, IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _repository = repository;
            _context = context;
        }

        public override Task<List<VmTicket>> GetList()
        {
            //return base.GetList();
            //var exist = await _repository.GetFilter(a => a.VersionFull == version);
            //var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
            return _repository.GetListFilter(a => a.IsDeleted != true);
        }
    }
}
