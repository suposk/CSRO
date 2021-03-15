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
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<TicketDto>(content, _options);
                    var result = Mapper.Map<Ticket>(ser);
                    return result;
                }
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
                {
                    return true;
                }
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
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return false;
        }

        public async Task<Ticket> GetItemByIdAsync(int id)
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}{id}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<TicketDto>(content, _options);
                    var result = Mapper.Map<Ticket>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<List<Ticket>> GetItemsAsync()
        {
            try
            {
                await base.AddAuthHeaderAsync();

                var url = $"{ApiPart}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<TicketDto>>(content, _options);
                    var result = Mapper.Map<List<Ticket>>(ser);
                    return result;
                }
                else
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    if (string.IsNullOrWhiteSpace(content)) content = apiData.ReasonPhrase;
                    throw new Exception(content);
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
                throw;
            }
        }

        public Task<List<Ticket>> GetItemsByParrentIdAsync(int parrentId)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetItemsByTypeAsync(string type)
        {
            throw new NotImplementedException();
        }
    }
}
