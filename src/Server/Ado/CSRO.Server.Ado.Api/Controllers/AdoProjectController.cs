﻿using AutoMapper;
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

        // GET: api/MessageDetails/5        
        [HttpGet("{id}", Name = nameof(GetRequestAdoProject))]
        public async Task<ActionResult<ProjectAdo>> GetRequestAdoProject(int id)
        {
            if (id < 1)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.GetItem, $"{nameof(GetRequestAdoProject)} with {id} Started");

                var repoObj = await _repository.GetId(id).ConfigureAwait(false);
                if (repoObj == null)
                    return NotFound();

                //_mapper.Map(repoObj, result);
                var result = _mapper.Map<ProjectAdo>(repoObj);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(GetRequestAdoProject), id);
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
        }

        [HttpPost, Route(nameof(RequestAdoProject))]        
        public async Task<ActionResult<ProjectAdo>> RequestAdoProject(ProjectAdo dto)
        {
            if (dto == null)
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.RequestItem, $"{nameof(RequestAdoProject)} Started");

                var repoObj = _mapper.Map<Entity.AdoProject>(dto);              
                var suc = await _repository.CreateAdoProject(repoObj);
                if (suc != null)
                {
                    var result = _mapper.Map<ProjectAdo>(repoObj);                    
                    return CreatedAtRoute(nameof(GetRequestAdoProject),
                        new { id = result.Id }, result);
                }
                else
                    return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(RequestAdoProject), dto);                
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }               
        }

        // POST api/<AdoProjectController>
        [HttpPost, Route(nameof(ApproveAdoProject))]
        public async Task<ActionResult<List<ProjectAdo>>> ApproveAdoProject(List<int> toApprove)
        {
            if (toApprove == null || !toApprove.Any())
                return BadRequest();

            try
            {
                _logger.LogInformation(ApiLogEvents.ApproveItem, $"{nameof(ApproveAdoProject)} Started");                
                var approved = await _repository.ApproveAdoProject(toApprove).ConfigureAwait(false);
                var result = _mapper.Map<List<ProjectAdo>>(approved);
                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, nameof(ApproveAdoProject), toApprove);                
                return StatusCode(StatusCodes.Status500InternalServerError, ex?.Message);
            }
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
