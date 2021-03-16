using CSRO.Server.Entities.Entity;
using CSRO.Server.Services.Ado;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Ado.Api.Services
{
    public class AdoProjectApproverService : IAdoProjectApproverService
    {
        public Task<List<AdoProjectApprover>> GetAdoProjectApprovers()
        {
            List<AdoProjectApprover> list = new();
            list.Add(new AdoProjectApprover { UserId = "suposk", Email = "suposk@yahoo.com", ApproverType = ApproverType.Primary });
            list.Add(new AdoProjectApprover { UserId = "jan.supolik", Email = "jan.supolik@gmail.com", ApproverType = ApproverType.Deputy });
            return Task.FromResult(list);
        }
    }
}