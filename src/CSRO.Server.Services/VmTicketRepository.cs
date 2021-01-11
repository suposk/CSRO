using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace CSRO.Server.Services
{
    public interface IVmTicketRepository : IRepository<VmTicket>
    {
        Task<VmTicket> CreateRestartTicket(VmTicket entity);
    }

    public class VmTicketRepository : Repository<VmTicket>, IVmTicketRepository
    {
        private readonly IAzureVmManagementService _azureVmManagementService;
        private readonly IRepository<VmTicket> _repository;
        private AppVersionContext _context;
        private string _userId;

        public VmTicketRepository(
            IAzureVmManagementService azureVmManagementService,
            IRepository<VmTicket> repository, 
            AppVersionContext context, 
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _azureVmManagementService = azureVmManagementService;
            _repository = repository;
            _context = context;
            _userId = ApiIdentity.GetUserName();
        }
        public async Task<VmTicket> CreateRestartTicket(VmTicket entity)
        {            
            try
            {
                var vmstatus = await _azureVmManagementService.GetVmDisplayStatus(entity);
                if (vmstatus.suc == false || vmstatus.status.Contains("deallocat"))
                    throw new Exception($"Unable to process request: {vmstatus.status}");

                var sent = await _azureVmManagementService.RestarVmInAzure(entity);
                if (!sent.suc)
                    throw new Exception(sent.errorMessage);

                base.Add(entity, _userId);
                entity.Status = "Opened";
                entity.VmState = "Restart Started";
            }
            catch
            {
                throw;
            }
            return entity;
        }

        public async override void Add(VmTicket entity, string UserId = null)
        {
            await CreateRestartTicket(entity);
            //base.Add(entity, _userId);
            //entity.Status = "Opened";
            //entity.VmState = "Restart Started";
        }

        public override void Update(VmTicket entity, string UserId = null)
        {
            entity.Status = "Modified";
            base.Update(entity, _userId);
        }

        public override void Remove(VmTicket entity, string UserId = null)
        {
            base.Remove(entity, _userId);
            entity.Status = "Closed";
        }

        //public override Task<List<VmTicket>> GetList()
        //{
        //    //return base.GetList();
        //    //var exist = await _repository.GetFilter(a => a.VersionFull == version);
        //    //var q = _context.AppVersions.Where(e => !_context.AppVersions.Any(e2 => e2.VersionValue > e.VersionValue));
        //    return _repository.GetListFilter(a => a.IsDeleted != true);
        //}
    }
}
