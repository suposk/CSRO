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
        private readonly ITicketRepository _ticketRepository;
        private readonly IMapper _mapper;

        public TicketController(ILogger<TicketController> logger,
            //IRepository<Ticket> repository,
            ITicketRepository ticketRepository,
            IMapper mapper)
        {
            _logger = logger;
            //_repository = repository;
            _ticketRepository = ticketRepository;
            _mapper = mapper;
        }

        // GET: api/<VersionController>
        [HttpGet]
        public async Task<ActionResult<List<TicketDto>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");

                var all = await _ticketRepository.GetAllAsync();
                var result = _mapper.Map<List<TicketDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
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

                var repoObj = await _ticketRepository.GetAsync(id);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                result = _mapper.Map<TicketDto>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetTicket), null);
                throw;
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
                _ticketRepository.Add(repoObj);
                if (await _ticketRepository.SaveChangesAsync())
                {
                    var result = _mapper.Map<TicketDto>(repoObj);
                    return CreatedAtRoute("GetTicket",
                        new { id = result.Id }, result);
                }
                
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(PostTicket), null);
                throw;
            }
            return null;
        }
    }
}
