using AutoMapper;
using CSRO.Server.Api.Commands;
using CSRO.Server.Domain;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using CSRO.Server.Services;
using MediatR;
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
        private readonly IMediator _mediator;
        private readonly IMapper _mapper;

        public VmTicketController(ILogger<VmTicketController> logger,            
            IVmTicketRepository repository,
            IMediator mediator,
            IMapper mapper)
        {
            _logger = logger;            
            _repository = repository;
            _mediator = mediator;
            _mapper = mapper;
        }

        // GET: api/<VersionController>
        [HttpGet]
        public async Task<ActionResult<List<VmTicketDto>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");

                var all = await _repository.GetList().ConfigureAwait(false);
                var result = _mapper.Map<List<VmTicketDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Get), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        // GET: api/MessageDetails/5        
        [HttpGet("{id}", Name = nameof(GetVmTicketById))]
        public async Task<ActionResult<VmTicketDto>> GetVmTicketById(int id)
        {
            if (id < 1)
                return BadRequest();
                        
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetVmTicketById)} with {id} Started");

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                var result = _mapper.Map<VmTicketDto>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetVmTicketById), id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        //// POST: api/MessageDetails
        [HttpPost, Route(nameof(CreateRestartTicket))]        
        public async Task<ActionResult<VmTicketDto>> CreateRestartTicket(VmTicketDto dto)
        {
            if (dto == null)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.InsertItem, $"{nameof(CreateRestartTicket)} Started");

                var repoObj = _mapper.Map<VmTicket>(dto);
                var wmOperationRequestCommand = new VmOperationRequestCommand() { VmTicket = repoObj };
                var responseMessage = await _mediator.Send(wmOperationRequestCommand);
                if (!responseMessage.Success)
                    //throw new Exception(responseMessage.Message);
                    return StatusCode(StatusCodes.Status409Conflict, responseMessage.Message);
                else
                {
                    var result = _mapper.Map<VmTicketDto>(responseMessage.ReturnedObject);
                    return CreatedAtRoute(nameof(GetVmTicketById),
                        new { id = result.Id }, result);
                }

                //await _repository.CreateRestartTicket(repoObj).ConfigureAwait(false);
                //if (await _repository.SaveChangesAsync())
                //{
                //    var result = _mapper.Map<VmTicketDto>(repoObj);
                //    return CreatedAtRoute(nameof(GetVmTicketById),
                //        new { id = result.Id }, result);
                //}
                //else
                //    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(CreateRestartTicket), dto);
                //return StatusCode(StatusCodes.Status500InternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }               
        }

        //// POST: api/MessageDetails
        [HttpPost, Route(nameof(RebootVmAndWaitForConfirmation))]
        public async Task<ActionResult<VmTicketDto>> RebootVmAndWaitForConfirmation(VmTicketDto dto)
        {
            if (dto == null)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.InsertItem, $"{nameof(RebootVmAndWaitForConfirmation)} Started");

                var repoObj = _mapper.Map<VmTicket>(dto);
                var res = await _repository.RebootVmAndWaitForConfirmation(repoObj).ConfigureAwait(false);
                if (res.success)
                {
                    var result = _mapper.Map<VmTicketDto>(repoObj);
                    return CreatedAtRoute(nameof(GetVmTicketById),
                        new { id = result.Id }, result);
                }
                else
                    return BadRequest(res.errorMessage);

            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RebootVmAndWaitForConfirmation), dto);
                //return StatusCode(StatusCodes.Status500InternalServerError);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
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

                var repoObj = await _repository.GetId(dto.Id).ConfigureAwait(false);
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
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
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

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
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
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
            return null;
        }
    }
}
