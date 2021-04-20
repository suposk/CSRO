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
                    return null;

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersBySubNames), null);
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
                    return null;

                var result = _mapper.Map<List<CustomerDto>>(all);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetCustomersBySubIds), null);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }
    }
}
