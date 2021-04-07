using CSRO.Client.Services.Models;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IAdoProjectAccessDataService : IBaseDataService<AdoProjectAccessModel>
    {
        Task<List<AdoProjectAccessModel>> ApproveAdoProject(List<int> toApprove);
        Task<List<AdoProjectAccessModel>> RejectAdoProject(List<int> toReject, string rejectReason);
        Task<List<AdoProjectAccessModel>> GetProjectsForApproval();
    }
}
