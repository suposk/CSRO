using AutoMapper;
using CSRO.Common.AdoServices.Models;
using CSRO.Server.Ado.Api.Services;
using CSRO.Server.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSRO.Server.Ado.Api.Controllers
{
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

        // GET api/<AdoProjectController>/5
        [HttpGet("{id}")]
        public string Get(int id)
        {
            return "value " + id;
        }

        // POST api/<AdoProjectController>
        [HttpPost]
        public void Post([FromBody] string value)
        {

        }

        //// PUT api/<AdoProjectController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<AdoProjectController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
