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
using CSRO.Server.Ado.Api.Commands;
using MediatR;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSRO.Server.Ado.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class AdoProjectAccessController : ControllerBase
    {
        private readonly ILogger<AdoProjectAccessController> _logger;
        private readonly IAdoProjectAccessRepository _repository;
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public AdoProjectAccessController(ILogger<AdoProjectAccessController> logger,
            IAdoProjectAccessRepository repository,
            IMediator mediator,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mediator = mediator;
            _mapper = mapper;
        }

        // GET: api/<AdoProjectAccessController>
        [HttpGet]
        public async Task<ActionResult<List<AdoProjectAccessDto>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");
                var all = await _repository.GetList().ConfigureAwait(false);
                var result = _mapper.Map<List<AdoProjectAccessDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Get), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        // GET api/<AdoProjectAccessController>/5
        [HttpGet("{id}", Name = nameof(GetRequestAdoProjectAccess))]        
        //[HttpGet("GetRequestAdoProjectAccess/{id}")]        
        public async Task<ActionResult<AdoProjectAccessDto>> GetRequestAdoProjectAccess(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetRequestAdoProjectAccess)} with {id} Started");

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                var result = _mapper.Map<AdoProjectAccessDto>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetRequestAdoProjectAccess), id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpGet("GetByUserId/{userId}")]        
        public async Task<ActionResult<List<AdoProjectAccessDto>>> GetByUserId(string userId)
        {
            if (string.IsNullOrWhiteSpace(userId))
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetByUserId)} Started");                
                var all = await _repository.GetByUserId(userId);
                var result = _mapper.Map<List<AdoProjectAccessDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetByUserId), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpPost, Route(nameof(RequestAdoProjectAccess))]
        public async Task<ActionResult<AdoProjectAccessDto>> RequestAdoProjectAccess(AdoProjectAccessDto dto)
        {
            if (dto == null)
                return BadRequest();

            if (dto.Id > 0)
                return BadRequest($"{dto.Id} may not be entered");

            try
            {
                _logger.LogInformation(ApiLogEvents.RequestItem, $"{nameof(RequestAdoProjectAccess)} Started");

                var repoObj = _mapper.Map<Entity.AdoProjectAccess>(dto);
                var suc = await _repository.CreateAdoProjectAccess(repoObj);
                if (suc != null)
                {
                    var result = _mapper.Map<AdoProjectAccessDto>(repoObj);
                    return CreatedAtRoute(nameof(GetRequestAdoProjectAccess),
                        new { id = result.Id }, result);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RequestAdoProjectAccess), dto);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        // POST api/<AdoProjectController>
        [Authorize(Policy = Core.PoliciesCsro.CanApproveAdoRequestPolicy)]
        //[Authorize(Roles = "Admin")]
        [HttpPost, Route(nameof(ApproveAdoProjectAccess))]
        public async Task<ActionResult<List<AdoProjectAccessDto>>> ApproveAdoProjectAccess(List<int> toApprove)
        {
            if (toApprove == null || !toApprove.Any())
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.ApproveItem, $"{nameof(ApproveAdoProjectAccess)} Started");
                var approveRejectAdoProjectsAccessCommand = new ApproveRejectAdoProjectAccessIdsCommand() { IdsList = toApprove, Reject = false };
                var approved = await _mediator.Send(approveRejectAdoProjectsAccessCommand);
                //var approved = await _repository.ApproveRejectAdoProjects(toApprove, false, null).ConfigureAwait(false);
                var result = _mapper.Map<List<AdoProjectAccessDto>>(approved);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ApproveAdoProjectAccess), toApprove);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [Authorize(Policy = Core.PoliciesCsro.CanApproveAdoRequestPolicy)]
        [HttpPost, Route(nameof(RejectAdoProjectAccess))]
        public async Task<ActionResult<List<AdoProjectAccessDto>>> RejectAdoProjectAccess(RejectededListDto toReject)
        {
            if (toReject == null || toReject.ToReject.IsNullOrEmptyCollection())
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.ApproveItem, $"{nameof(RejectAdoProjectAccess)} Started");
                //var rejected = await _repository.ApproveRejectAdoProjects(toReject.ToReject, true, toReject.Reason).ConfigureAwait(false);
                var approveRejectAdoProjectsAccessCommand = new ApproveRejectAdoProjectAccessIdsCommand() { IdsList = toReject.ToReject, Reject = true, Reason = toReject.Reason };
                var rejected = await _mediator.Send(approveRejectAdoProjectsAccessCommand);
                var result = _mapper.Map<List<AdoProjectAccessDto>>(rejected);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RejectAdoProjectAccess), toReject);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        //// POST api/<AdoProjectAccessController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<AdoProjectAccessController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteAdoProjectAccessRequest(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.DeleteItem, $"{nameof(DeleteAdoProjectAccessRequest)} Started");

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
                if (repoObj == null)
                {
                    _logger.LogWarning(ApiLogEvents.DeleteItemNotFound, $"{nameof(DeleteAdoProjectAccessRequest)} not found");
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
                _logger.LogError(ex, nameof(DeleteAdoProjectAccessRequest), id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
            return null;
        }
    }
}
