using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CSRO.Server.Services
{
    public interface IVmTicketHistoryRepository : IRepository<VmTicketHistory>
    {
        Task<VmTicketHistory> Create(VmTicket vmTicket);
        Task<List<VmTicketHistory>> GetHitoryByParentId(int parentId);
    }

    public class VmTicketHistoryRepository : Repository<VmTicketHistory>, IVmTicketHistoryRepository
    {

        private readonly IRepository<VmTicketHistory> _repository;
        private readonly AppVersionContext _context;
        private readonly string _userId;

        public VmTicketHistoryRepository(
            IRepository<VmTicketHistory> repository,
            AppVersionContext context,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _repository = repository;
            _context = context;
            _userId = ApiIdentity.GetUserName();
        }

        public Task<List<VmTicketHistory>> GetHitoryByParentId(int parentId)
        {
            return _context.VmTicketHistories.Where(a => a.IsDeleted != true && a.VmTicketId == parentId).OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public async Task<VmTicketHistory> Create(VmTicket vmTicket)
        {            
            var add = new VmTicketHistory { VmTicketId = vmTicket.Id, Operation = $"{vmTicket.VmState} {vmTicket.Status}" };
            _repository.Add(add, _userId);
            try
            {
                await _repository.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                add.Details = ex?.Message;
                await(_repository.SaveChangesAsync());                
            }
            return add;
        }
    }
}
