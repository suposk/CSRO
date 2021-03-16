using CSRO.Common.AdoServices.Models;
using CSRO.Server.Entities;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace CSRO.Server.Ado.Api.Services
{
    public interface IAdoProjectHistoryRepository: IRepository<AdoProjectHistory>
    {
        const string Operation_SentEmailForApproval = "SentEmailForApproval";

        Task<AdoProjectHistory> Create(string operation, string userId);
        Task<List<AdoProject>> GetPendingProjectsApproval();
    }

    public class AdoProjectHistoryRepository : Repository<AdoProjectHistory>, IAdoProjectHistoryRepository
    {        

        private readonly IRepository<AdoProjectHistory> _repository;
        private readonly AdoContext _context;

        public AdoProjectHistoryRepository(
            IRepository<AdoProjectHistory> repository,
            AdoContext context,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {
            _repository = repository;
            _context = context;
            //_userId = ApiIdentity.GetUserName();
        }

        public override Task<List<AdoProjectHistory>> GetList()
        {
            return _repository.GetListFilter(a => a.IsDeleted != true);
        }

        public async Task<List<AdoProject>> GetPendingProjectsApproval()
        {
            var q = _context.AdoProjectHistorys.Where(
                a => a.AdoProject.IsDeleted != true && 
                a.AdoProject.State == ProjectState.CreatePending && (a.Operation == IAdoProjectHistoryRepository.Operation_SentEmailForApproval)
                );
            var all = await q.ToListAsync();            
            return all.IsNullOrEmptyCollection()? null : all.Select(a=> a.AdoProject).ToList();
        }

        public async Task<AdoProjectHistory> Create(string operation, string userId)
        {
            if (string.IsNullOrEmpty(operation))            
                throw new ArgumentException($"'{nameof(operation)}' cannot be null or empty.", nameof(operation));            

            if (string.IsNullOrEmpty(userId))            
                throw new ArgumentException($"'{nameof(userId)}' cannot be null or empty.", nameof(userId));
            
            var entity = new AdoProjectHistory { Operation = operation };
            base.Add(entity, userId);
            await SaveChangesAsync();
            return entity;            
        }
    }
}
