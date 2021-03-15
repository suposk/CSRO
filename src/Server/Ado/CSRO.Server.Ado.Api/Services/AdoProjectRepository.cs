using AutoMapper;
using CSRO.Common.AdoServices;
using CSRO.Common.AdoServices.Models;
using CSRO.Server.Entities;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using CSRO.Server.Services;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api.Services
{
    public interface IAdoProjectRepository : IRepository<AdoProject>
    {
        Task<List<AdoProject>> ApproveAdoProject(List<int> toApprove);
        Task<AdoProject> CreateAdoProject(AdoProject entity);
    }

    public class AdoProjectRepository : Repository<AdoProject>, IAdoProjectRepository
    {        
        private readonly IRepository<AdoProject> _repository;
        private AdoContext _context;                
        private readonly IProjectAdoServices _projectAdoServices;
        private readonly IEmailService _emailService;
        private readonly IMapper _mapper;
        private string _userId;        

        public AdoProjectRepository(            
            IRepository<AdoProject> repository,
            AdoContext context,                        
            IProjectAdoServices projectAdoServices,
            IEmailService emailService,
            IMapper mapper,
            IApiIdentity apiIdentity) : base(context, apiIdentity)
        {            
            _repository = repository;
            _context = context;                        
            _projectAdoServices = projectAdoServices;
            _emailService = emailService;
            _mapper = mapper;
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
                base.Add(entity, _userId);
                ////test only
                //await _emailService.Send("jan.supolik@hotmail.com", "suposk@yahoo.com", $"test subject service at {DateTime.Now}", $"tested at {DateTime.Now}", false);

                await SaveChangesAsync();
                return entity;
            }
            catch
            {
                throw;
            }           
        }

        public async Task<List<AdoProject>> ApproveAdoProject(List<int> toApprove)
        {
            if (toApprove is null)
                throw new ArgumentNullException(nameof(toApprove));

            try
            {                         
                List<AdoProject> approved = new();
                StringBuilder others = new();

                foreach (var pId in toApprove)
                {

                    try
                    {
                        var entity = await _repository.GetId(pId);
                        if (entity != null)
                        {
                            //only if in pending state
                            if (entity.State != ProjectState.CreatePending)
                                continue;

                            //1. create Proj
                            var mapped = _mapper.Map<ProjectAdo>(entity);                           
                            var created = await _projectAdoServices.CreateProject(mapped);

                            //2. Update Db
                            entity = _mapper.Map<AdoProject>(created);
                            //entity.State = ProjectState.New;
                            base.Update(entity, _userId);
                            if (await SaveChangesAsync())
                                approved.Add(entity);
                        }
                        else
                            others.Append($"Id {pId} was not found, verify this Id exist or record was modified.");
                    }
                    catch (Exception ex)
                    {
                        others.Append($"Error approving Id {pId}: {ex.Message}");
                    }
                }
                return approved.Any() ? approved : null;
            }
            catch
            {
                throw;
            }            
        }

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
