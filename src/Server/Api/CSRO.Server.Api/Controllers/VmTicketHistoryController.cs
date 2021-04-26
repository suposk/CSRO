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
    [Route("api/VmTicket/{VmTicketId}/VmTicketHistory")]
    [ApiController]
    public class VmTicketHistoryController : ControllerBase
    {
        private readonly ILogger<VmTicketHistoryController> _logger;
        private readonly IVmTicketHistoryRepository _repository;
        private readonly IMapper _mapper;

        public VmTicketHistoryController(ILogger<VmTicketHistoryController> logger,
            IVmTicketHistoryRepository repository,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        [HttpGet(Name = nameof(GetVmTicketHistory))]
        public async Task<ActionResult<List<VmTicketHistoryDto>>> GetVmTicketHistory(int VmTicketId)
        {
            if (VmTicketId < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetVmTicketHistory)} with {VmTicketId} Started");              
                var repoObj = await _repository.GetHitoryByParentId(VmTicketId).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();
                
                var result = _mapper.Map<List<VmTicketHistoryDto>>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetVmTicketHistory), VmTicketId);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

    }
}
