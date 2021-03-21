using CSRO.Server.Entities.Entity;
using MediatR;
using System.Collections.Generic;

namespace CSRO.Server.Ado.Api.Commands
{
    public class CreateApprovedAdoProjectIdsCommand : IRequest<List<AdoProject>>
    {
        public List<int> Approved { get; set; }

        public string UserId { get; set; }
    }
}
