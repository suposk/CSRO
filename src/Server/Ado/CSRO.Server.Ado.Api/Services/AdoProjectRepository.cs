using CSRO.Server.Entities;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api.Services
{
    public interface IAdoProjectRepository : IRepository<AdoProject>
    {
        Task<AdoProject> CreateAdoProject(AdoProject entity);
    }

    public class AdoProjectRepository : Repository<AdoProject>, IAdoProjectRepository
    {        
        private readonly IRepository<AdoProject> _repository;
        private AdoContext _context;        
        private readonly IConfiguration _configuration;
        private string _userId;        

        public AdoProjectRepository(            
            IRepository<AdoProject> repository,
            AdoContext context,            
            IConfiguration configuration,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {            
            _repository = repository;
            _context = context;            
            _configuration = configuration;
            _userId = ApiIdentity.GetUserName();            
        }

        public override Task<List<AdoProject>> GetList()
        {
            return _repository.GetListFilter(a => a.IsDeleted != true);
        }

        public async Task<AdoProject> CreateAdoProject(AdoProject entity)
        {
            try
            {
                //var vmstatus = await _azureVmManagementService.GetVmDisplayStatus(entity);
                //if (vmstatus.suc == false || vmstatus.status.Contains("deallocat"))
                //    throw new Exception($"Unable to process request: {vmstatus.status}");

                //var sent = await _azureVmManagementService.RestarVmInAzure(entity).ConfigureAwait(false);
                //if (!sent.suc)
                //    throw new Exception(sent.errorMessage);

                base.Add(entity, _userId);                         
            }
            catch
            {
                throw;
            }
            return entity;
        }

        //public async Task<List<AdoProject>> ApproveAdoProject(List<int> approved)
        //{
        //    try
        //    {
        //        //var vmstatus = await _azureVmManagementService.GetVmDisplayStatus(entity);
        //        //if (vmstatus.suc == false || vmstatus.status.Contains("deallocat"))
        //        //    throw new Exception($"Unable to process request: {vmstatus.status}");

        //        //var sent = await _azureVmManagementService.RestarVmInAzure(entity).ConfigureAwait(false);
        //        //if (!sent.suc)
        //        //    throw new Exception(sent.errorMessage);

        //        base.Update(entity, _userId);
        //    }
        //    catch
        //    {
        //        throw;
        //    }
        //    return entity;
        //}

        public override void Add(AdoProject entity, string UserId = null)
        {            
            base.Add(entity, _userId);
            entity.State = ProjectState.CreatePending;            
        }

        public override void Remove(AdoProject entity, string UserId = null)
        {
            base.Remove(entity, _userId);
            entity.State = ProjectState.Deleted;
        }
    }
}
