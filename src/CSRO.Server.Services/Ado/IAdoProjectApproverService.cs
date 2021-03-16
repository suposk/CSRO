using CSRO.Server.Entities.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSRO.Server.Services.Ado
{
    public interface IAdoProjectApproverService
    {
        Task<List<AdoProjectApprover>> GetAdoProjectApprovers();
    }
}
