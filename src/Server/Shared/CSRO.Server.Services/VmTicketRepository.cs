using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using CSRO.Common.AzureSdkServices;
using Microsoft.Extensions.Configuration;
using CSRO.Server.Services.AzureRestServices;
using CSRO.Server.Entities.Enums;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
        private readonly IVmSdkService _vmSdkService;
        private readonly IConfiguration _configuration;
        private string _userId;
        private readonly int VmRebootDelay = 0;

        public VmTicketRepository(
            IAzureVmManagementService azureVmManagementService,
            IRepository<VmTicket> repository, 
            AppVersionContext context,
            IVmSdkService vmSdkService,
            IConfiguration configuration,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _azureVmManagementService = azureVmManagementService;
            _repository = repository;
            _context = context;
            _vmSdkService = vmSdkService;
            _configuration = configuration;
            _userId = ApiIdentity.GetUserName();

            VmRebootDelay = configuration.GetValue<int>("VmRebootDelay");
        }

        public override Task<List<VmTicket>> GetList()
        {
            //return _repository.GetListFilter(a => a.IsDeleted != true);
            return _context.VmTickets.Where(a => a.IsDeleted != true).OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public override void Add(VmTicket entity, string UserId = null)
        {            
            base.Add(entity, _userId);
            entity.Status = Status.Submitted.ToString(); 
            entity.VmState = $"{entity.Operation} Submitted";
        }

        public override void Remove(VmTicket entity, string UserId = null)
        {
            base.Remove(entity, _userId);
            entity.Status = Status.Deleted.ToString(); 
        }
    }
}
