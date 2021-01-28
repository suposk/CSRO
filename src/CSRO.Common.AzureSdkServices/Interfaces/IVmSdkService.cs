using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Common.AzureSdkServices
{
    public interface IVmSdkService
    {        
        Task<List<object>> TryGetData(string subscriptionId, string resourceGroupName, string vmName);
    }
}