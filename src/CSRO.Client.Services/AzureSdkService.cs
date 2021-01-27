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
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public interface IAzureSdkService
    {
        Task<List<object>> TryGetData(string subscriptionId, string resourceGroupName, string vmName);
    }

    public class AzureSdkService : IAzureSdkService
    {
        private readonly ICsroTokenCredentialProvider _csroTokenCredentialProvider;

        public AzureSdkService(ICsroTokenCredentialProvider csroTokenCredentialProvider)
        {
            _csroTokenCredentialProvider = csroTokenCredentialProvider;
        }

        public async Task<List<object>> TryGetData(string subscriptionId, string resourceGroupName, string vmName)
        {
            Response<VirtualMachineInstanceView> result = null;
            try
            {
                //var computeClient = new ComputeManagementClient(subscriptionId, new DefaultAzureCredential());
                //var networkClient = new NetworkManagementClient(subscriptionId, new DefaultAzureCredential());
                //var resourcesClient = new ResourcesManagementClient(subscriptionId, new DefaultAzureCredential());

                var computeClient = new ComputeManagementClient(subscriptionId, _csroTokenCredentialProvider.GetCredential());
                var networkClient = new NetworkManagementClient(subscriptionId, _csroTokenCredentialProvider.GetCredential());
                var resourcesClient = new ResourcesManagementClient(subscriptionId, _csroTokenCredentialProvider.GetCredential());

                var virtualNetworksClient = networkClient.VirtualNetworks;
                var networkInterfaceClient = networkClient.NetworkInterfaces;
                //var publicIpAddressClient = networkClient.PublicIPAddressses;
                var publicIpAddressClient = networkClient.PublicIPAddresses;
                var availabilitySetsClient = computeClient.AvailabilitySets;
                var virtualMachinesClient = computeClient.VirtualMachines;
                var resourceGroupClient = resourcesClient.ResourceGroups;
                
                var list = new List<object>();

                result = await virtualMachinesClient.InstanceViewAsync(resourceGroupName, vmName);
                var resp = result.GetRawResponse();                
                list.Add(result);
                
                AsyncPageable<VirtualMachine> vmPages = virtualMachinesClient.ListAllAsync();
                await foreach (VirtualMachine item in vmPages)
                {
                    list.Add(result.Value.Statuses);
                }

                AsyncPageable<Subscription> subs = resourcesClient.Subscriptions.ListAsync();
                await foreach(var sub in subs)
                {
                    list.Add(sub);
                    
                }

                return list;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }



}
