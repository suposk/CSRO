using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using System.Threading.Tasks;
using System.Collections.Generic;
using CSRO.Server.Entities;

namespace CSRO.Server.Services
{
    public interface ICustomerRepository
    {
        CustomersDbContext Context { get; }
        Task<List<ResourceSWI>> GetCustomersBySubNames(List<string> subscriptionNames);
        Task<List<ResourceSWI>> GetCustomersBySubIds(List<string> subscriptionIds);
        Task<List<ResourceSWI>> GetCustomersByRegion(List<string> regions);
    }
}
