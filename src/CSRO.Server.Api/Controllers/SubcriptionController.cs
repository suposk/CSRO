using CSRO.Server.Domain;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
        // GET: api/<SubcriptionController>
        [HttpGet]
        public IEnumerable<IdNameDto> Get()
        {
            return new IdNameDto[] 
            {
                new IdNameDto { Id = Guid.NewGuid().ToString(), Name = "value 1" },
                new IdNameDto { Id = Guid.NewGuid().ToString(), Name = "value 2" },
            };
        }

        //// GET api/<SubcriptionController>/5
        //[HttpGet("{id}")]
        //public string Get(int id)
        //{
        //    return "value";
        //}
    }
}
