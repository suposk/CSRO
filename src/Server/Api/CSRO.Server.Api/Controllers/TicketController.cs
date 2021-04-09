using AutoMapper;
using CSRO.Common.AzureSdkServices;
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
    public class TicketController : ControllerBase
    {
        private readonly ILogger<TicketController> _logger;
        //private readonly IRepository<Ticket> _repository;
        private readonly ITicketRepository _repository;
        private readonly IMapper _mapper;

        public TicketController(ILogger<TicketController> logger,
            ITicketRepository repository,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        // GET: api/<VersionController>
        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");

                //throw new Exception("some fake value is null");

                var all = await _repository.GetList().ConfigureAwait(false);
                var result = _mapper.Map<List<TicketDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Get), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        // GET: api/MessageDetails/5        
        [HttpGet("{id}", Name = nameof(GetTicket))]
        //public async Task<ActionResult<TicketDto>> GetTicket(int id)
        public async Task<ActionResult<TicketDto>> GetTicket(int id, [FromServices] IAdService adService)
        {
            if (id < 1)
                return BadRequest();            
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetTicket)} with {id} Started");

                //var ad = await adService.GetCurrentAdUserInfo();

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                var result = _mapper.Map<TicketDto>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetTicket), id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        //// POST: api/MessageDetails
        [HttpPost]
        public async Task<ActionResult<TicketDto>> PostTicket(TicketDto dto)
        {
            if (dto == null)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.InsertItem, $"{nameof(PostTicket)} Started");

                var repoObj = _mapper.Map<Ticket>(dto);                
                _repository.Add(repoObj);
                if (await _repository.SaveChangesAsync())
                {
                    var result = _mapper.Map<TicketDto>(repoObj);
                    return CreatedAtRoute(nameof(GetTicket),
                        new { id = result.Id }, result);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(PostTicket), dto);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
            return null;
        }

        // PUT: api/Churchs/5
        [HttpPut()]
        public async Task<ActionResult<TicketDto>> PutTicket(TicketDto dto)
        {
            if (dto == null || dto.Id < 1)
                return BadRequest();
                        
            try
            {
                _logger.LogInformation(ApiLogEvents.UpdateItem, $"{nameof(PutTicket)} Started");

                var repoObj = await _repository.GetId(dto.Id).ConfigureAwait(false);
                if (repoObj == null)
                {
                    _logger.LogWarning(ApiLogEvents.UpdateItemNotFound, $"{nameof(PutTicket)} not found");
                    return NotFound();
                }

                repoObj = _mapper.Map<Ticket>(dto);
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
                _logger.LogError(ex, nameof(PutTicket), dto);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }            
        }

        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteTicket(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.DeleteItem, $"{nameof(DeleteTicket)} Started");

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
                if (repoObj == null)
                {
                    _logger.LogWarning(ApiLogEvents.DeleteItemNotFound, $"{nameof(DeleteTicket)} not found");
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
                _logger.LogError(ex, nameof(DeleteTicket), id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
            return null;
        }
    }
}
