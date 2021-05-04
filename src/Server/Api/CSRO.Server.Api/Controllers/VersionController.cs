using AutoMapper;
using CSRO.Server.Api.Dtos;
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
    [Authorize]
    //[AutoValidateAntiforgeryToken]
    [Route("api/[controller]")]
    [ApiController]
    public class VersionController : ControllerBase
    {
        private readonly ILogger<VersionController> _logger;
        private readonly IRepository<AppVersion> _repository;
        private readonly IVersionRepository _versionRepository;
        private readonly IMapper _mapper;

        public VersionController(ILogger<VersionController> logger,
            IRepository<AppVersion> repository,
            IVersionRepository versionRepository,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _versionRepository = versionRepository;
            _mapper = mapper;
        }

        // GET: api/<VersionController>
        [HttpGet]
        public async Task<ActionResult<List<AppVersionDto>>> Get()
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(Get)} Started");

                var all = await _repository.GetList();
                var result = _mapper.Map<List<AppVersionDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogErrorCsro(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        // GET api/<VersionController>/5
        [HttpGet("{version}", Name = nameof(GetVersion))]
        public async Task<ActionResult<AppVersionDto>> GetVersion(string version)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetVersion)} Started");

                AppVersionDto result = null;
                var res = await _versionRepository.GetVersion(version);
                result = _mapper.Map<AppVersionDto>(res);
                return result;

            }
            catch (Exception ex)
            {
                _logger.LogErrorCsro(ex);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
    }
}
