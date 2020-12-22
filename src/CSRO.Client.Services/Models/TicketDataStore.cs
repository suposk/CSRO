using AutoMapper;
using CSRO.Client.Services;
using Microsoft.Identity.Web;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services.Models
{
    public class TicketDataStore : IBaseDataStore<Ticket>
    {
        const string _apiPart = "api/version/";
        const string scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
        JsonSerializerOptions _options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };

        private readonly IHttpClientFactory _httpClientFactory;
        private readonly ITokenAcquisition _tokenAcquisition;
        private readonly IMapper _mapper;
        private HttpClient _httpClient;

        public TicketDataStore(IHttpClientFactory httpClientFactory, ITokenAcquisition tokenAcquisition, IMapper mapper)
        {
            _httpClientFactory = httpClientFactory;
            _tokenAcquisition = tokenAcquisition;
            _mapper = mapper;
            if (_httpClient == null)
                _httpClient = _httpClientFactory.CreateClient("api");
        }

        public Task<bool> AddItemAsync(Ticket item)
        {
            throw new NotImplementedException();
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<Ticket> GetItemByIdAsync(int id)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Ticket>> GetItemsAsync(bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Ticket>> GetItemsByParrentIdAsync(int parrentId, bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<IList<Ticket>> GetItemsByTypeAsync(Enum type, bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<bool> UpdateItemAsync(Ticket item)
        {
            throw new NotImplementedException();
        }
    }
}
