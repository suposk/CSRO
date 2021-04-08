using CSRO.Server.Entities.Entity;
using MediatR;
using System.Collections.Generic;

namespace CSRO.Server.Ado.Api.Commands
{
    public class ApproveRejectAdoProjectAccessIdsCommand : IRequest<List<AdoProjectAccess>>
    {
        public List<int> IdsList { get; set; }
        public bool Reject { get; set; }
        public string Reason { get; set; }

        //public string UserId { get; set; }
    }
}
