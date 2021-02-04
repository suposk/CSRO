using CSRO.Server.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSRO.Server.Core.Helpers;

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
                list.Add($"HelloSetting = {helloSetting}");

                bool UseKeyVault = _configuration.GetValue<bool>("UseKeyVault");
                list.Add($"{nameof(UseKeyVault)} = {UseKeyVault}");

                string TokenCacheDbConnStr = _configuration.GetConnectionString("TokenCacheDbConnStr");
                list.Add($"{nameof(TokenCacheDbConnStr)} = {TokenCacheDbConnStr.ReplaceWithStars()}");

                string SqlConnString = _configuration.GetConnectionString("SqlConnString");
                list.Add($"{nameof(SqlConnString)} = {SqlConnString.ReplaceWithStars()}");

                string ClientSecretVaultName = _configuration.GetValue<string>("ClientSecretVaultName");
                list.Add($"{nameof(ClientSecretVaultName)} = {ClientSecretVaultName}");

                string ClientSecret = _configuration.GetValue<string>("AzureAd:ClientSecret");
                list.Add($"{nameof(ClientSecret)} = {ClientSecret.ReplaceWithStars()}");

                var VaultName = _configuration.GetValue<string>("CsroVaultNeuDev");
                list.Add($"{nameof(VaultName)} = {VaultName}");

                return list;
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError);
            }
        }

        // GET api/<VersionController>/5
        [HttpGet("{id}", Name = nameof(GetSquereOf))]
        public async Task<ActionResult<string>> GetSquereOf(int id)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetSquereOf)} Started");
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
