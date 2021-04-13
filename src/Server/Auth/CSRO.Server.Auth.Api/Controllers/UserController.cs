using AutoMapper;
using Entity = CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CSRO.Server.Services;
using CSRO.Server.Auth.Api.Dtos;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSRO.Server.Auth.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly ILogger<UserController> _logger;
        private readonly ILocalUserService _localUserService;
        private readonly IMapper _mapper;

        public UserController(ILogger<UserController> logger,
            ILocalUserService localUserService,
            IMapper mapper)
        {
            _logger = logger;
            _localUserService = localUserService;
            _mapper = mapper;
        }

        // GET: api/<UserController>
        [HttpGet]
        public IEnumerable<string> Get()
        {
            return new string[] { "user 1", "user 2" };
        }

        // GET api/<AdoProjectAccessController>/5
        [HttpGet("{userName}", Name = nameof(GetUserbyUserName))]        
        public async Task<ActionResult<UserDto>> GetUserbyUserName(string userName)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetUserbyUserName)} with {userName} Started");

                var repoObj = await _localUserService.GetUserByUserNameAsync(userName).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();
                                
                var result = _mapper.Map<UserDto>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetUserbyUserName), userName);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        //// POST api/<UserController>
        //[HttpPost]
        //public void Post([FromBody] string value)
        //{
        //}

        //// PUT api/<UserController>/5
        //[HttpPut("{id}")]
        //public void Put(int id, [FromBody] string value)
        //{
        //}

        //// DELETE api/<UserController>/5
        //[HttpDelete("{id}")]
        //public void Delete(int id)
        //{
        //}
    }
}
