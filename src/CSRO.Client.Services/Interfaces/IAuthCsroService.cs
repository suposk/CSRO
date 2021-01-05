using System.Collections.Generic;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IAuthCsroService
    {
        Task<string> GetAccessTokenForUserAsync(List<string> scopes);
        Task<string> GetAccessTokenForUserAsync(string scope);
    }
}