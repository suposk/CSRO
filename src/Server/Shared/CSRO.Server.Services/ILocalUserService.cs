using CSRO.Server.Entities.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Services
{
    public interface ILocalUserService
    {
        Task<List<UserClaim>> GetUserClaimsByUserNameAsync(string userName);
        Task<List<Claim>> GetClaimsByUserNameAsync(string userName);        
        Task<User> GetUserByUserNameAsync(string userName);
        void AddUser(User userToAdd, string password);
        Task<bool> SaveChangesAsync();
    }
}
