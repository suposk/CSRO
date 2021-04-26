using AutoMapper;
using CSRO.Server.Api.Services;
using CSRO.Server.Domain;
using CSRO.Server.Infrastructure;
using CSRO.Server.Services;
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
    public class CustomerController : ControllerBase
    {
        private readonly ILogger<CustomerController> _logger;
        private readonly ICustomerRepository _repository;
        private readonly IMapper _mapper;

        public CustomerController(
            ILogger<CustomerController> logger,
            ICustomerRepository repository,
            IMapper mapper)
        {
            _logger = logger;
            _repository = repository;
            _mapper = mapper;
        }
                
        //[HttpGet]
        //public IEnumerable<string> Get()
        //{
        //    return new string[] { "cus 1", "cus 2" };
        //}
                
        [HttpGet(nameof(GetCustomersBySubNames))]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersBySubNames(List<string> subscriptionNames)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetCustomersBySubNames)} Started");

                var all = await _repository.GetCustomersBySubNames(subscriptionNames).ConfigureAwait(false);
                if (all.IsNullOrEmptyCollection())
                    return new List<CustomerDto>();

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersBySubNames), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
                
        [HttpGet("GetCustomersBySubName/{subscriptionName}")]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersBySubName(string subscriptionName)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetCustomersBySubName)} Started");

                var all = await _repository.GetCustomersBySubName(subscriptionName).ConfigureAwait(false);
                if (all.IsNullOrEmptyCollection())
                    return new List<CustomerDto>();

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersBySubName), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpGet(nameof(GetCustomersBySubIds))]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersBySubIds(List<string> subscriptionIds)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetCustomersBySubIds)} Started");

                var all = await _repository.GetCustomersBySubIds(subscriptionIds).ConfigureAwait(false);
                if (all.IsNullOrEmptyCollection())
                    return new List<CustomerDto>();

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersBySubIds), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
                
        [HttpGet("GetCustomersBySubId/{subscriptionId}")]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersBySubId(string subscriptionId)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetCustomersBySubId)} Started");

                var all = await _repository.GetCustomersBySubId(subscriptionId).ConfigureAwait(false);
                if (all.IsNullOrEmptyCollection())
                    return new List<CustomerDto>();

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersBySubId), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpGet(nameof(GetCustomersByRegions))]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersByRegions(List<string> regions)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetCustomersByRegions)} Started");

                var all = await _repository.GetCustomersByRegions(regions).ConfigureAwait(false);
                if (all.IsNullOrEmptyCollection())
                    return new List<CustomerDto>();

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersByRegions), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpGet(nameof(GetCustomersByAtCodes))]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersByAtCodes(List<string> atCodes)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetCustomersByAtCodes)} Started");

                var all = await _repository.GetCustomersByAtCodes(atCodes).ConfigureAwait(false);
                if (all.IsNullOrEmptyCollection())
                    return new List<CustomerDto>();

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersByAtCodes), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpGet("GetCustomersByAtCode/{atCode}")]
        public async Task<ActionResult<List<CustomerDto>>> GetCustomersByAtCode(string atCode)
        {
            try
            {
                _logger.LogInformation(ApiLogEvents.GetAllItems, $"{nameof(GetCustomersByAtCode)} Started");

                var all = await _repository.GetCustomersByAtCode(atCode).ConfigureAwait(false);
                if (all.IsNullOrEmptyCollection())
                    return new List<CustomerDto>();

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersByAtCode), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

    }
}
