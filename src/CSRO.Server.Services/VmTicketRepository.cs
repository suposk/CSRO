using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using System;
using CSRO.Common.AzureSdkServices;

namespace CSRO.Server.Services
{
    public interface IVmTicketRepository : IRepository<VmTicket>
    {
        Task<VmTicket> CreateRestartTicket(VmTicket entity);
        Task<(bool success, string errorMessage)> RebootVmAndWaitForConfirmation(VmTicket entity);
    }

    public class VmTicketRepository : Repository<VmTicket>, IVmTicketRepository
    {
        private readonly IAzureVmManagementService _azureVmManagementService;
        private readonly IRepository<VmTicket> _repository;
        private AppVersionContext _context;
        private readonly IVmSdkService _vmSdkService;
        private string _userId;

        public VmTicketRepository(
            IAzureVmManagementService azureVmManagementService,
            IRepository<VmTicket> repository, 
            AppVersionContext context,
            IVmSdkService vmSdkService,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _azureVmManagementService = azureVmManagementService;
            _repository = repository;
            _context = context;
            _vmSdkService = vmSdkService;
            _userId = ApiIdentity.GetUserName();
        }

        public override Task<List<VmTicket>> GetList()
        {
            return _repository.GetListFilter(a => a.IsDeleted != true);
        }

        public async Task<VmTicket> CreateRestartTicket(VmTicket entity)
        {            
            try
            {
                var vmstatus = await _azureVmManagementService.GetVmDisplayStatus(entity);
                if (vmstatus.suc == false || vmstatus.status.Contains("deallocat"))
                    throw new Exception($"Unable to process request: {vmstatus.status}");

                var sent = await _azureVmManagementService.RestarVmInAzure(entity).ConfigureAwait(false);
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

        public async Task<(bool success, string errorMessage)> RebootVmAndWaitForConfirmation(VmTicket entity)
        {
            try
            {
                //first add
                base.Add(entity, _userId);
                entity.Status = "Opened";
                entity.VmState = "Restart Submited";
                await SaveChangesAsync();
                
                var res = await _vmSdkService.RebootVmAndWaitForConfirmation(entity.SubcriptionId, entity.ResorceGroup, entity.VmName).ConfigureAwait(false);
                //update
                base.Update(entity, _userId);                                
                if (res.success)
                {                    
                    entity.Status = "Completed";
                    entity.VmState = res.status;
                }
                else
                {
                    entity.Status = "Rejected";
                    entity.VmState = res.errorMessage;
                }
                await SaveChangesAsync();
                return (res.success, res.errorMessage);                
            }
            catch(Exception ex)
            {
                return (false, $"Unable to RebootVm: \n{ex.Message}");
            }            
        }

        public override void Add(VmTicket entity, string UserId = null)
        {
            //await CreateRestartTicket(entity);
            base.Add(entity, _userId);
            entity.Status = "Opened";
            entity.VmState = "Restart Started";
        }

        public override void Remove(VmTicket entity, string UserId = null)
        {
            base.Remove(entity, _userId);
            entity.Status = "Closed";
        }
    }
}
