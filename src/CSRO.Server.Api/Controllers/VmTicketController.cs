using AutoMapper;
using CSRO.Server.Domain;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using CSRO.Server.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Api.Controllers
{
    [Authorize]
    //[AutoValidateAntiforgeryToken]
    [Route("api/[controller]")]
    [ApiController]
    public class VmTicketController : ControllerBase
    {
        private readonly ILogger<VmTicketController> _logger;
        //private readonly IRepository<Vm> _repository;
        private readonly IVmTicketRepository _repository;
        private readonly IMapper _mapper;

        public VmTicketController(ILogger<VmTicketController> logger,            
            IVmTicketRepository repository,
            IMapper mapper)
        {
            _logger = logger;            
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/<VersionController>
        [HttpGet]
        public async Task<ActionResult<List<VmTicketDto>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");

                var all = await _repository.GetList();
                var result = _mapper.Map<List<VmTicketDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Get), null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/MessageDetails/5        
        [HttpGet("{id}", Name = "GetById")]
        public async Task<ActionResult<VmTicketDto>> GetById(int id)
        {
            if (id < 1)
                return BadRequest();
                        
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetById)} with {id} Started");

                var repoObj = await _repository.GetId(id);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                var result = _mapper.Map<VmTicketDto>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetById), id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        //// POST: api/MessageDetails
        [HttpPost]
        public async Task<ActionResult<VmTicketDto>> PostVmTicket(VmTicketDto dto)
        {
            if (dto == null)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.InsertItem, $"{nameof(PostVmTicket)} Started");

                var repoObj = _mapper.Map<VmTicket>(dto);                
                _repository.Add(repoObj);
                if (await _repository.SaveChangesAsync())
                {
                    var result = _mapper.Map<VmTicketDto>(repoObj);
                    return CreatedAtRoute("GetById",
                        new { id = result.Id }, result);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(PostVmTicket), dto);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return null;
        }

        // PUT: api/Churchs/5
        [HttpPut()]
        public async Task<ActionResult<VmTicketDto>> PutVmTicket(VmTicketDto dto)
        {
            if (dto == null || dto.Id < 1)
                return BadRequest();
                        
            try
            {
                _logger.LogInformation(ApiLogEvents.UpdateItem, $"{nameof(PutVmTicket)} Started");

                var repoObj = await _repository.GetId(dto.Id);
                if (repoObj == null)
                {
                    _logger.LogWarning(ApiLogEvents.UpdateItemNotFound, $"{nameof(PutVmTicket)} not found");
                    return NotFound();
                }

                repoObj = _mapper.Map<VmTicket>(dto);
                _repository.Update(repoObj);
                if (await _repository.SaveChangesAsync())
                {
                    return NoContent();
                }
                else
                {
                    return Conflict("Conflict detected, refresh and try again.");
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(PutVmTicket), dto);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }            
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteVmTicket(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.DeleteItem, $"{nameof(DeleteVmTicket)} Started");

                var repoObj = await _repository.GetId(id);
                if (repoObj == null)
                {
                    _logger.LogWarning(ApiLogEvents.DeleteItemNotFound, $"{nameof(DeleteVmTicket)} not found");
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
                _logger.LogError(ex, nameof(DeleteVmTicket), id);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return null;
        }
    }
}
