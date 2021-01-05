using AutoMapper;
using CSRO.Client.Services;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
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
    public class TicketDataStore : BaseDataStore, IBaseDataStore<Ticket>
    {
        public TicketDataStore(IHttpClientFactory httpClientFactory, IAuthCsroService authCsroService, IMapper mapper)
            : base(httpClientFactory, authCsroService, mapper)
        {
            ApiPart = "api/ticket/";
            Scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
            ClientName = "api";

            base.Init();
        }

        public async Task<Ticket> AddItemAsync(Ticket item)
        {
            try
            {
                await base.AddAuthHeader();

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

        public async Task<bool> DeleteItemAsync(int id)
        {
            try
            {
                await base.AddAuthHeader();

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
                await base.AddAuthHeader();

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

        public async Task<List<Ticket>> GetItemsAsync(bool forceRefresh = false)
        {
            try
            {
                await base.AddAuthHeader();

                var url = $"{ApiPart}";
                var apiData = await HttpClientBase.GetAsync(url).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<List<TicketDto>>(content, _options);
                    var result = Mapper.Map<List<Ticket>>(ser);
                    return result;
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public Task<List<Ticket>> GetItemsByParrentIdAsync(int parrentId, bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public Task<List<Ticket>> GetItemsByTypeAsync(Enum type, bool forceRefresh = false)
        {
            throw new NotImplementedException();
        }

        public async Task<bool> UpdateItemAsync(Ticket item)
        {
            try
            {
                await base.AddAuthHeader();

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
    }
}
