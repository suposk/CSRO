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

        public async Task<Ticket> AddItemAsync(Ticket item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var add = Mapper.Map<TicketDto>(item);
                var httpcontent = new StringContent(JsonSerializer.Serialize(add, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PostAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<TicketDto>(stream, _options);
                    var result = Mapper.Map<Ticket>(ser);
                    return result;
                }
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<bool> UpdateItemAsync(Ticket item)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var add = Mapper.Map<TicketDto>(item);
                var httpcontent = new StringContent(JsonSerializer.Serialize(add, _options), Encoding.UTF8, "application/json");
                var apiData = await HttpClientBase.PutAsync(url, httpcontent).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)                
                    return true;                
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.DeleteAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    return true;
                }
                else
                    throw new Exception(GetErrorText(apiData));
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
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
