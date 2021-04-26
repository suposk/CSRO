using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{
    public class VmTicketHistoryDataService : BaseDataService, IBaseDataService<VmTicketHistory>
    {
        public VmTicketHistoryDataService(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            //[Route("api/VmTicket/{VmTicketId}/VmTicketHistory")]
            ApiPart = "api/VmTicket/{VmTicketId}/VmTicketHistory";            
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpoint;
            base.Init();
        }

        public Task<List<VmTicketHistory>> GetItemsByParrentIdAsync(int parrentId)
        {
            ApiPart = $"api/VmTicket/{parrentId}/VmTicketHistory";
            return base.RestGetListById<VmTicketHistory, VmTicketHistoryDto>(null);
        }

        public Task<VmTicketHistory> AddItemAsync(VmTicketHistory item)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(VmTicketHistory item)
        {
            throw new System.NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<VmTicketHistory> GetItemByIdAsync(int id)
        {
            throw new System.NotImplementedException();
        }

        public Task<List<VmTicketHistory>> GetItemsAsync()
        {
            throw new System.NotImplementedException();
        }
    }
}
