using AutoMapper;
using CSRO.Server.Api.Services;
using CSRO.Server.Domain;
using CSRO.Server.Infrastructure;
using CSRO.Server.Services.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSRO.Server.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class SubcriptionController : ControllerBase
    {
        private readonly ILogger<SubcriptionController> _logger;
        private readonly ISubcriptionRepository _repository;
        private readonly IMapper _mapper;

        public SubcriptionController(
            ILogger<SubcriptionController> logger,
            ISubcriptionRepository repository,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }
        

        [HttpGet]        
        //[ResponseCache(Duration = Core.ConstatCsro.CacheSettings.DefaultDuration, Location = ResponseCacheLocation.Any, NoStore = false)] not working
        public async Task<ActionResult<List<IdNameDto>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");

                var all = await _repository.GetSubcriptions().ConfigureAwait(false);
                if (all.IsNullOrEmptyCollection())
                    return null;

                var result = _mapper.Map<List<IdNameDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(Get), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
                
        [HttpGet("{subscriptionIds}", Name = nameof(GetTags))]        
        public async Task<ActionResult<List<CustomerDto>>> GetTags(List<string> subscriptionIds)
        {
            try
            {                
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetTags)} Started");
                return null;

                //var all = await _repository.GetTags(subscriptionIds).ConfigureAwait(false);
                //if (all.IsNullOrEmptyCollection())
                //    return null;

                //var result = _mapper.Map<List<CustomerDto>>(all);
                //return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetTags), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        //// GET api/<SubcriptionController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}
    }
}
