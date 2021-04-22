using AutoMapper;
using CSRO.Client.Services;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using CSRO.Client.Core;
using CSRO.Client.Services.AzureRestServices;

namespace CSRO.Client.Services
{
    public interface IVmTicketDataService : IBaseDataService<VmTicket>
    {
        /// <summary>
        /// Call directly azure api and updated status
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        Task<bool> VerifyRestartStatus(VmTicket item);
        Task<bool> VerifyRestartStatusCallback(VmTicket item, Action<string> callbackStatus);        
        Task<VmTicket> RebootVmAndWaitForConfirmation(VmTicket item);
    }

    public class VmTicketDataService : BaseDataService, IVmTicketDataService
    {
        private readonly IVmService _vmManagementService;

        public VmTicketDataService(
            IVmService vmManagementService, 
            IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper, IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/vmticket/";
            //Scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpoint;

            base.Init();

            _vmManagementService = vmManagementService;
        }


        public async Task<bool> VerifyRestartStatus(VmTicket item)
        {
            
            try
            {
                var vmstatus = await _vmManagementService.GetVmDisplayStatus(item);
                if (vmstatus.suc)
                {
                    var server = await GetItemByIdAsync(item.Id);
                    if (server == null)
                        return false;

                    server.VmState = vmstatus.status;
                    var up = await UpdateItemAsync(server);
                    if (server.VmState.ToLower().Contains("running"))
                    {
                        item = server;
                        return true;
                    }
                }                   
                
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

        public async Task<bool> VerifyRestartStatusCallback(VmTicket item, Action<string> callbackStatus)
        {

            try
            {
                var i = 1;
                while (i < 10)
                {
                    i++;
                    await Task.Delay(2 * 1000);

                    var vmstatus = await _vmManagementService.GetVmDisplayStatus(item);
                    if (vmstatus.suc)
                    {
                        var server = await GetItemByIdAsync(item.Id);
                        if (server == null)
                            return false;

                        server.VmState = vmstatus.status;
                        if (server.VmState.ToLower().Contains("running"))
                        {
                            server.Status = "Completed";
                            var up = await UpdateItemAsync(server);
                            item = server;
                            return true;
                        }
                        else
                            callbackStatus?.Invoke(vmstatus.status);
                    }
                }

            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

        public Task<VmTicket> RebootVmAndWaitForConfirmation(VmTicket item)
        {
            return base.RestAdd<VmTicket, VmTicketDto>(item, "RebootVmAndWaitForConfirmation");
        }

        // generic methods

        public Task<VmTicket> AddItemAsync(VmTicket item)
        {
            return base.RestAdd<VmTicket, VmTicketDto>(item, "CreateRestartTicket");
        }

        public Task<bool> UpdateItemAsync(VmTicket item)
        {
            return base.RestUpdate<VmTicket, VmTicketDto>(item);
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            return base.RestDeleteById(id);
        }

        public Task<VmTicket> GetItemByIdAsync(int id)
        {
            return base.RestGetById<VmTicket, VmTicketDto>(id.ToString());
        }

        public Task<List<VmTicket>> GetItemsAsync()
        {
            return base.RestGetListById<VmTicket, VmTicketDto>();
        }
    }
}
