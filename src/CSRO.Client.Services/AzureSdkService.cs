﻿using Azure;
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

namespace CSRO.Client.Services
{
    public interface IAzureSdkService
    {
        Task<List<Models.IdName>> GetAllSubcriptions(string subscriptionId);
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
                TokenCredential tokenCredential = null;
                tokenCredential = _csroTokenCredentialProvider.GetCredential();

                //var computeClient = new ComputeManagementClient(subscriptionId, new DefaultAzureCredential());
                //var networkClient = new NetworkManagementClient(subscriptionId, new DefaultAzureCredential());
                //var resourcesClient = new ResourcesManagementClient(subscriptionId, new DefaultAzureCredential());

                var computeClient = new ComputeManagementClient(subscriptionId, tokenCredential);
                //var networkClient = new NetworkManagementClient(subscriptionId, tokenCredential);
                var resourcesClient = new ResourcesManagementClient(subscriptionId, tokenCredential);

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

        public async Task<List<Models.IdName>> GetAllSubcriptions(string subscriptionId)
        {            
            try
            {
                TokenCredential tokenCredential = _csroTokenCredentialProvider.GetCredential();
                var resourcesClient = new ResourcesManagementClient(subscriptionId, tokenCredential);
                var resourceGroupClient = resourcesClient.ResourceGroups;                
                var result = new List<Models.IdName>();

                AsyncPageable<Subscription> subs = resourcesClient.Subscriptions.ListAsync();
                await foreach (var sub in subs)
                {
                    result.Add(new Models.IdName { Id = sub.SubscriptionId, Name = sub.DisplayName });
                }
                return result;
            }
            catch (Exception ex)
            {

            }
            return null;
        }
    }



}
