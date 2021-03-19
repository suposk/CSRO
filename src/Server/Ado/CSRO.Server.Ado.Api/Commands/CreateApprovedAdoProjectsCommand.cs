using CSRO.Server.Entities.Entity;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api.Commands
{
    public class CreateApprovedAdoProjectsCommand : IRequest<List<AdoProject>>
    {
        public List<AdoProject> Approved { get; set; }

        public string UserId { get; set; }
    }
}
