﻿using CSRO.Server.Infrastructure;
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

        public async override void Add(VmTicket entity, string UserId = null)
        {
            var vmstatus = await _azureVmManagementService.GetVmDisplayStatus(entity);

            base.Add(entity, _userId);
            entity.Status = "Opened";
            entity.VmState = "Restart Started";
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
