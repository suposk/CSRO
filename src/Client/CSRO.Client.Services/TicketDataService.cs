using AutoMapper;
using CSRO.Client.Core;
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

namespace CSRO.Client.Services
{
    public class TicketDataService : BaseDataService, IBaseDataService<Ticket>
    {
        public TicketDataService(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper, 
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/ticket/";
            //Scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";            
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Api);            
            ClientName = ConstatCsro.EndPoints.ApiEndpoint;
            base.Init();
        }

        public Task<Ticket> AddItemAsync(Ticket item)
        {
            return base.RestAdd<Ticket, TicketDto>(item);
        }

        public Task<bool> UpdateItemAsync(Ticket item)
        {
            return base.RestUpdate<Ticket, TicketDto>(item);
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            return base.RestDeleteById(id);
        }

        public Task<Ticket> GetItemByIdAsync(int id)
        {
            return base.RestGetById<Ticket, TicketDto>(id.ToString());
        }

        public Task<List<Ticket>> GetItemsAsync()
        {
            return base.RestGetListById<Ticket, TicketDto>();
        }
    }
}
