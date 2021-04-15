using AutoMapper;
using CSRO.Server.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using CSRO.Server.Services;
using CSRO.Server.Auth.Api.Dtos;
using CSRO.Server.Domain;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace CSRO.Server.Auth.Api.Controllers
{
    [Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class UserClaimController : ControllerBase
    {
        private readonly ILogger<UserClaimController> _logger;
        private readonly ILocalUserService _localUserService;
        private readonly IMapper _mapper;

        public UserClaimController(
            ILogger<UserClaimController> logger,
            ILocalUserService localUserService,
            IMapper mapper)
        {
            _logger = logger;
            _localUserService = localUserService;
            _mapper = mapper;
        }

        //// GET: api/<UserClaimController>
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "UserClaim 1", "UserClaim 2" };
        //}

        // GET api/<AdoProjectAccessController>/5
        [HttpGet("{userName}", Name = nameof(GetUserClaimsByUserName))]
        public async Task<ActionResult<List<UserClaimDto>>> GetUserClaimsByUserName(string userName)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetUserClaimsByUserName)} with {userName} Started");

                var repoObj = await _localUserService.GetUserClaimsByUserNameAsync(userName).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();

                var result = _mapper.Map<List<UserClaimDto>>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetUserClaimsByUserName), userName);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
    }
}
