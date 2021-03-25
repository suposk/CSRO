using AutoMapper;
using CSRO.Common.AdoServices.Models;
using CSRO.Server.Ado.Api.Services;
using Entity = CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRO.Server.Ado.Api.Dtos;
using Microsoft.AspNetCore.Authorization;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSRO.Server.Ado.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdoProjectController : ControllerBase
    {

        private readonly ILogger<AdoProjectController> _logger;
        //private readonly IRepository<Vm> _repository;
        private readonly IAdoProjectRepository _repository;
        private readonly IMapper _mapper;

        public AdoProjectController(ILogger<AdoProjectController> logger,
            IAdoProjectRepository repository,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/<AdoProjectController>
        public async Task<ActionResult<List<ProjectAdo>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");                
                var all = await _repository.GetList().ConfigureAwait(false);
                var result = _mapper.Map<List<ProjectAdo>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Get), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        // GET: api/MessageDetails/5        
        [HttpGet("{id}", Name = nameof(GetRequestAdoProject))]
        public async Task<ActionResult<ProjectAdo>> GetRequestAdoProject(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetRequestAdoProject)} with {id} Started");

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                var result = _mapper.Map<ProjectAdo>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetRequestAdoProject), id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
                
        [HttpGet("{organization}/{projectName}/{projectId}", Name = nameof(ProjectExists))]
        public async Task<ActionResult<bool>> ProjectExists(string organization, string projectName, int projectId)
        {
            if (string.IsNullOrWhiteSpace(projectName) || string.IsNullOrWhiteSpace(organization))
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(ProjectExists)} with {projectName} {organization} Started");

                var res = await _repository.ProjectExists(organization, projectName, projectId).ConfigureAwait(false);
                return res ? Ok(true) : NotFound();                    
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ProjectExists), projectName, organization, projectId);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpPost, Route(nameof(SaveDraftAdoProject))]
        public async Task<ActionResult<ProjectAdo>> SaveDraftAdoProject(ProjectAdo dto)
        {
            if (dto == null)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.RequestItem, $"{nameof(SaveDraftAdoProject)} Started");

                var repoObj = _mapper.Map<Entity.AdoProject>(dto);
                _repository.Add(repoObj);
                var suc = await _repository.SaveChangesAsync();
                if (suc)
                {
                    var result = _mapper.Map<ProjectAdo>(repoObj);
                    return CreatedAtRoute(nameof(GetRequestAdoProject),
                        new { id = result.Id }, result);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(SaveDraftAdoProject), dto);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }


        [HttpPost, Route(nameof(RequestAdoProject))]        
        public async Task<ActionResult<ProjectAdo>> RequestAdoProject(ProjectAdo dto)
        {
            if (dto == null)
                return BadRequest();

            if (dto.Id > 0)
                return BadRequest($"{dto.Id} may not be entered");

            try
            {
                _logger.LogInformation(ApiLogEvents.RequestItem, $"{nameof(RequestAdoProject)} Started");

                var repoObj = _mapper.Map<Entity.AdoProject>(dto);              
                var suc = await _repository.CreateAdoProject(repoObj);
                if (suc != null)
                {
                    var result = _mapper.Map<ProjectAdo>(repoObj);                    
                    return CreatedAtRoute(nameof(GetRequestAdoProject),
                        new { id = result.Id }, result);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RequestAdoProject), dto);                
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }               
        }

        // POST api/<AdoProjectController>
        [Authorize(Policy = Core.PoliciesCsro.CanApproveAdoRequest)]
        [HttpPost, Route(nameof(ApproveAdoProject))]
        public async Task<ActionResult<List<ProjectAdo>>> ApproveAdoProject(List<int> toApprove)
        {
            if (toApprove == null || !toApprove.Any())
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.ApproveItem, $"{nameof(ApproveAdoProject)} Started");
                //var approved = await _repository.ApproveAndCreateAdoProjects(toApprove).ConfigureAwait(false);
                var approved = await _repository.ApproveRejectAdoProjects(toApprove, false, null).ConfigureAwait(false);
                var result = _mapper.Map<List<ProjectAdo>>(approved);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ApproveAdoProject), toApprove);                
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpPost, Route(nameof(RejectAdoProject))]
        public async Task<ActionResult<List<ProjectAdo>>> RejectAdoProject(RejectededListDto toReject)
        {
            if (toReject == null || toReject.ToReject.IsNullOrEmptyCollection())
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.ApproveItem, $"{nameof(RejectAdoProject)} Started");                
                var approved = await _repository.ApproveRejectAdoProjects(toReject.ToReject, true, toReject.Reason).ConfigureAwait(false);
                var result = _mapper.Map<List<ProjectAdo>>(approved);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RejectAdoProject), toReject);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        //// PUT api/<AdoProjectController>/5
        [HttpPut()]
        public async Task<ActionResult<ProjectAdo>> UpdateAdoProjectRequest(ProjectAdo dto)
        {
            if (dto == null || dto.Id < 1)
                return BadRequest();

            if (dto.Status > Status.Submitted)
                return BadRequest($"Can not Modify request if {nameof(dto.Status)} is {dto.Status}. Please Create new request.");

            try
            {
                _logger.LogInformation(ApiLogEvents.UpdateItem, $"{nameof(UpdateAdoProjectRequest)} Started");

                var repoObj = await _repository.GetId(dto.Id).ConfigureAwait(false);
                if (repoObj == null)
                {
                    _logger.LogWarning(ApiLogEvents.UpdateItemNotFound, $"{nameof(UpdateAdoProjectRequest)} not found");
                    return NotFound();
                }

                repoObj = _mapper.Map<Entity.AdoProject>(dto);
                var res = await _repository.UpdateAsync(repoObj).ConfigureAwait(false);
                if (res != null)                
                    return NoContent();                
                else                
                    return Conflict("Conflict detected, refresh and try again.");                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(UpdateAdoProjectRequest), dto);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }




        // DELETE api/<AdoProjectController>/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdoProjectRequest(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.DeleteItem, $"{nameof(DeleteAdoProjectRequest)} Started");

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
                if (repoObj == null)
                {
                    _logger.LogWarning(ApiLogEvents.DeleteItemNotFound, $"{nameof(DeleteAdoProjectRequest)} not found");
                    return NotFound();
                }

                _repository.Remove(repoObj);
                if (await _repository.SaveChangesAsync())
                {
                    return NoContent();
                }

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(DeleteAdoProjectRequest), id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
            return null;
        }
    }
}
