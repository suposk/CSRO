using Azure;
using Azure.Core;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Common.AzureSdkServices
{
    public interface IVmSdkService
    {
        Task<(bool success, string status, string errorMessage)> RebootVmAndWaitForConfirmation(string subscriptionId, string resourceGroupName, string vmName, CancellationToken cancelToken = default);
        Task<List<object>> TryGetData(string subscriptionId, string resourceGroupName, string vmName);
    }

    public class VmSdkService : IVmSdkService
    {
        private readonly ICsroTokenCredentialProvider _csroTokenCredentialProvider;
        private readonly TokenCredential _tokenCredential;

        public VmSdkService(ICsroTokenCredentialProvider csroTokenCredentialProvider)
        {
            _csroTokenCredentialProvider = csroTokenCredentialProvider;
            _tokenCredential = _csroTokenCredentialProvider.GetCredential();
        }

        public async Task<List<object>> TryGetData(string subscriptionId, string resourceGroupName, string vmName)
        {
            Response<VirtualMachineInstanceView> result = null;
            try
            {                               

                //var computeClient = new ComputeManagementClient(subscriptionId, new DefaultAzureCredential());
                //var networkClient = new NetworkManagementClient(subscriptionId, new DefaultAzureCredential());
                //var resourcesClient = new ResourcesManagementClient(subscriptionId, new DefaultAzureCredential());

                var computeClient = new ComputeManagementClient(subscriptionId, _tokenCredential);
                //var networkClient = new NetworkManagementClient(subscriptionId, tokenCredential);
                var resourcesClient = new ResourcesManagementClient(subscriptionId, _tokenCredential);

                var availabilitySetsClient = computeClient.AvailabilitySets;
                var virtualMachinesClient = computeClient.VirtualMachines;
                //var virtualNetworksClient = networkClient.VirtualNetworks;
                //var networkInterfaceClient = networkClient.NetworkInterfaces;
                ////var publicIpAddressClient = networkClient.PublicIPAddressses;
                //var publicIpAddressClient = networkClient.PublicIPAddresses;
                //var resourceGroupClient = resourcesClient.ResourceGroups;

                var list = new List<object>();

                if (!string.IsNullOrWhiteSpace(resourceGroupName) && !string.IsNullOrWhiteSpace(vmName))
                {
                    result = await virtualMachinesClient.InstanceViewAsync(resourceGroupName, vmName);
                    var resp = result.GetRawResponse();
                    list.Add(result);
                }

                AsyncPageable<VirtualMachine> vmPages = virtualMachinesClient.ListAllAsync();
                await foreach (VirtualMachine item in vmPages)
                {
                    list.Add(result.Value.Statuses);
                }

                return list;
            }
            catch (Exception ex)
            {

            }
            return null;
        }

        public async Task<(bool success, string status, string errorMessage)> RebootVmAndWaitForConfirmation(string subscriptionId, string resourceGroupName, string vmName, CancellationToken cancelToken = default)
        {            
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionId) | string.IsNullOrWhiteSpace(resourceGroupName) || string.IsNullOrWhiteSpace(vmName))
                    return (false, null, $"Unable to {nameof(RebootVmAndWaitForConfirmation)}: missing parameters");

                var computeClient = new ComputeManagementClient(subscriptionId, _tokenCredential);
                var virtualMachinesClient = computeClient.VirtualMachines;

                var statusResults = new List<string>();
                Response<VirtualMachineInstanceView> result;

                result = await GetStatus(resourceGroupName, vmName, virtualMachinesClient, statusResults, cancelToken);
                //if (statusResults.Contains("deallocat"))
                if (statusResults.Any(a => a.Contains("deallocat")))
                    return (false, null, $"Unable to Reboot, Vm is {statusResults.FirstOrDefault() ?? "Stopped"}");

                var reb = await virtualMachinesClient.StartRestartAsync(resourceGroupName, vmName, cancelToken);
                if (reb != null)
                {
                    var up = await reb.UpdateStatusAsync(cancelToken);
                    result = await GetStatus(resourceGroupName, vmName, virtualMachinesClient, statusResults, cancelToken);
                    var wait = reb.WaitForCompletionAsync(cancelToken);
                    result = await GetStatus(resourceGroupName, vmName, virtualMachinesClient, statusResults, cancelToken);
                }
                return (true, statusResults.LastOrDefault(), null);
            }
            catch (Exception ex)
            {
                return (false, null, $"Unable to RebootVm: \n{ex.Message}");
            }

            static async Task<Response<VirtualMachineInstanceView>> GetStatus(string resourceGroupName, string vmName, VirtualMachinesOperations virtualMachinesClient, List<string> statuses, CancellationToken cancelToken)
            {
                Response<VirtualMachineInstanceView> result = await virtualMachinesClient.InstanceViewAsync(resourceGroupName, vmName, cancelToken);
                var last = result.Value.Statuses.LastOrDefault(a => a.Code.Contains("PowerState"));
                statuses.Add(last.DisplayStatus);
                return result;
            }
        }
    }
}