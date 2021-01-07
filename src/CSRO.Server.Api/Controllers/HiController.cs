using CSRO.Server.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Api.Controllers
{
    //[Authorize]
    [Route("api/[controller]")]
    [ApiController]
    public class HiController : ControllerBase
    {
        private readonly ILogger<HiController> _logger;
        private readonly IConfiguration _configuration;

        public HiController(
            ILogger<HiController> logger,
            IConfiguration configuration
            )
        {
            _logger = logger;
            _configuration = configuration;
        }

        // GET: api/<VersionController>
        [HttpGet]
        public async Task<ActionResult<List<string>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");
                List<string> list = new List<string>();
                var helloSetting = _configuration.GetValue<string>("HelloSetting");
                list.Add("Info");
                list.Add($"HelloSetting:{helloSetting}");
                return list;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET api/<VersionController>/5
        [HttpGet("{id}")]
        public async Task<ActionResult<string>> GetById(int id)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetById)} Started");
                var result = 0;
                try
                {
                    result = id * id;
                }
                finally { }
                return $"Squere of {id} = {result}";
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }


    }
}
