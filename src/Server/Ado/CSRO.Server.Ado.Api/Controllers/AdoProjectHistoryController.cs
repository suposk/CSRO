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
    //[Route("api/[controller]")]
    [Route("api/adoproject/{adoProjectId}/adoprojectHistory")]
    [ApiController]
    public class AdoProjectHistoryController : ControllerBase
    {

        private readonly ILogger<AdoProjectHistoryController> _logger;        
        private readonly IAdoProjectHistoryRepository _repository;
        private readonly IMapper _mapper;

        public AdoProjectHistoryController(ILogger<AdoProjectHistoryController> logger,
            IAdoProjectHistoryRepository repository,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }

        // GET api/<AdoProjectHistoryController>/5
        [HttpGet(Name = nameof(GetAdoProjectHistory))]
        public async Task<ActionResult<List<AdoProjectHistoryDto>>> GetAdoProjectHistory(int adoProjectId)
        {
            if (adoProjectId < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetAdoProjectHistory)} with {adoProjectId} Started");

                //var repoObj = await _repository.GetId(adoProjectId).ConfigureAwait(false);
                //var repoObj = await _repository.GetListFilter(a => a.IsDeleted != true && a.AdoProjectId == adoProjectId).ConfigureAwait(false);                
                var repoObj = await _repository.GetHitoryByParentId(adoProjectId).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                var result = _mapper.Map<List<AdoProjectHistoryDto>>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetAdoProjectHistory), adoProjectId);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
    }
}
