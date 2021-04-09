using Azure;
using Azure.Core;
using Azure.ResourceManager.Compute;
using Azure.ResourceManager.Compute.Models;
using Azure.ResourceManager.Network;
using Azure.ResourceManager.Resources;
using Azure.ResourceManager.Resources.Models;
using CSRO.Common.AzureSdkServices.Models;
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
        Task<IDictionary<string, string>> GetTags(string subscriptionId, string resourceGroupName, string vmName, CancellationToken cancelToken = default);
        Task<(bool success, string errorMessage)> IsRebootAllowed(string subscriptionId, string resourceGroupName, string vmName, CancellationToken cancelToken = default);
    }

    public class VmSdkService : IVmSdkService
    {
        private readonly ICsroTokenCredentialProvider _csroTokenCredentialProvider;
        private readonly IAdService _adService;
        private readonly ISubscriptionSdkService _subscriptionSdkService;
        private readonly TokenCredential _tokenCredential;

        public VmSdkService
            (
            ICsroTokenCredentialProvider csroTokenCredentialProvider,
            IAdService adService,
            ISubscriptionSdkService subscriptionSdkService
            )
        {
            _csroTokenCredentialProvider = csroTokenCredentialProvider;
            _adService = adService;
            _subscriptionSdkService = subscriptionSdkService;
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
                    return (false, null, $"{nameof(RebootVmAndWaitForConfirmation)}: missing parameters");

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
                return (false, null, $"Error in {nameof(RebootVmAndWaitForConfirmation)}: \n{ex.Message}");
            }

            static async Task<Response<VirtualMachineInstanceView>> GetStatus(string resourceGroupName, string vmName, VirtualMachinesOperations virtualMachinesClient, List<string> statuses, CancellationToken cancelToken)
            {
                Response<VirtualMachineInstanceView> result = await virtualMachinesClient.InstanceViewAsync(resourceGroupName, vmName, cancelToken);
                var last = result.Value.Statuses.LastOrDefault(a => a.Code.Contains("PowerState"));
                statuses.Add(last.DisplayStatus);
                return result;
            }
        }

        public async Task<(bool success, string errorMessage)> IsRebootAllowed(string subscriptionId, string resourceGroupName, string vmName, CancellationToken cancelToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionId) | string.IsNullOrWhiteSpace(resourceGroupName) || string.IsNullOrWhiteSpace(vmName))
                    return (false, $"missing parameters");

                var subTags = await _subscriptionSdkService.GetTags(subscriptionId, cancelToken);
                if (subTags?.Count == 0)
                    return (false, $"No tags on subscription");

                //first check subcription
                var opEnvironment = subTags.FirstOrDefault(a => a.Key.Equals(nameof(DefaultTagSdk.opEnvironment)));
                if (opEnvironment.Value == null || !opEnvironment.Value.Contains("dev", StringComparison.OrdinalIgnoreCase))
                    return (false, $"subscription must have {nameof(DefaultTagSdk.opEnvironment)} dev tag assign");

                //check VM
                var wmTags = await GetTags(subscriptionId, resourceGroupName, vmName, cancelToken);
                if (wmTags?.Count == 0)
                    return (false, $"No tags on vm");

                opEnvironment = wmTags.FirstOrDefault(a => a.Key.Equals(nameof(DefaultTagSdk.opEnvironment)));
                if (opEnvironment.Value == null || !opEnvironment.Value.Contains("dev", StringComparison.OrdinalIgnoreCase))
                    return (false, $"vm must have {nameof(DefaultTagSdk.opEnvironment)} dev tag assign");

                var privilegedMembers = wmTags.FirstOrDefault(a => a.Key.Equals(nameof(DefaultTagSdk.privilegedMembers)));
                if (privilegedMembers.Value == null)
                    return (false, $"No {nameof(DefaultTagSdk.privilegedMembers)} tags on wm assign");

                var adUserSdk = await _adService.GetCurrentAdUserInfo(includeGroups: false);
                //todo refactor. Must have value
                if (adUserSdk != null)
                {
                    if (string.IsNullOrWhiteSpace(adUserSdk.SamAccountName))
                        //posible exception
                        return (false, $"Failed to retrive {nameof(AdUserSdk.SamAccountName)}");

                    if (!privilegedMembers.Value.Contains(adUserSdk.SamAccountName, StringComparison.OrdinalIgnoreCase))
                        return (false, $"Your account {adUserSdk.SamAccountName} is not set on {nameof(DefaultTagSdk.privilegedMembers)} tags on vm");
                }
                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, $"Error in {nameof(IsRebootAllowed)}: \n{ex.Message}");
            }
        }

        public async Task<IDictionary<string, string>> GetTags(string subscriptionId, string resourceGroupName, string vmName, CancellationToken cancelToken = default)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(subscriptionId) | string.IsNullOrWhiteSpace(resourceGroupName) || string.IsNullOrWhiteSpace(vmName))
                    throw new Exception ($"{nameof(GetTags)}: missing parameters");

                var computeClient = new ComputeManagementClient(subscriptionId, _tokenCredential);
                var virtualMachinesClient = computeClient.VirtualMachines;

                var info = await virtualMachinesClient.GetAsync(resourceGroupName, vmName, cancelToken);
                return info?.Value.Tags;
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}