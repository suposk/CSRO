using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IAzureVmManagementService
    {
        //Task<(bool, AzureManagErrorDto)> RestarVmInAzure2(VmTicket item);
        Task<(bool suc, string errorMessage)> RestarVmInAzure(VmTicket item);
    }
}