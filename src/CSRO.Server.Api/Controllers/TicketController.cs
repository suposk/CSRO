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
    //[Authorize]
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
            //IRepository<Ticket> repository,
            ITicketRepository ticketRepository,
            IMapper mapper)
        {
            _logger = logger;
            //_repository = repository;
            _repository = ticketRepository;
            _mapper = mapper;
        }

        // GET: api/<VersionController>
        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");

                var all = await _repository.GetAllAsync();
                var result = _mapper.Map<List<TicketDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Get), null);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET: api/MessageDetails/5        
        [HttpGet("{id}", Name = "GetTicket")]
        public async Task<ActionResult<TicketDto>> GetTicket(int id)
        {
            if (id < 1)
                return BadRequest();

            TicketDto result = null;
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetTicket)} with {id} Started");

                var repoObj = await _repository.GetAsync(id);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                result = _mapper.Map<TicketDto>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetTicket), id);
                return StatusCode(StatusCodes.Status500InternalServerError);
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
                    return CreatedAtRoute("GetTicket",
                        new { id = result.Id }, result);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(PostTicket), dto);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return null;
        }

        // PUT: api/Churchs/5
        [HttpPut()]
        public async Task<ActionResult<TicketDto>> PutTicket(TicketDto dto)
        {
            if (dto == null || dto.Id < 1)
                return BadRequest();

            TicketDto result = null;
            try
            {
                _logger.LogInformation(ApiLogEvents.UpdateItem, $"{nameof(PutTicket)} Started");

                var repoObj = await _repository.GetAsync(dto.Id);
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
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(PutTicket), dto);
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return result;
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

                var repoObj = await _repository.GetAsync(id);
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
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
            return null;
        }
    }
}
