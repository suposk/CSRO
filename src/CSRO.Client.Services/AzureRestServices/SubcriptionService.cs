using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Core.Models;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Dtos.AzureDtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Client.Services.AzureRestServices
{
    public interface ISubcriptionService
    {
        Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default);

        Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default);

        Task<Subscription> GetSubcription(string subscriptionId, CancellationToken cancelToken = default);
        Task<List<TagNameWithValueList>> GetTags(string subscriptionId, CancellationToken cancelToken = default);
        Task<DefaultTags> GetDefualtTags(string subscriptionId, CancellationToken cancelToken = default);
        Task<Dictionary<string, DefaultTags>> GetDefualtTags(List<string> subscriptionIds, CancellationToken cancelToken = default);
        Task<List<Customer>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default);
    }

    public class SubcriptionService : BaseDataService, ISubcriptionService
    {
        //private readonly IAzureSdkService _azureSdkService;

        public SubcriptionService(
            IHttpClientFactory httpClientFactory,
            IAuthCsroService authCsroService,
            IMapper mapper,
            //IAzureSdkService azureSdkService,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            //_azureSdkService = azureSdkService;
            ApiPart = "--";
            //Scope = "api://ee2f0320-29c3-432a-bf84-a5d4277ce052/user_impersonation";
            Scope = ConstatCsro.Scopes.MANAGEMENT_AZURE_SCOPE;
            //ClientName = "api";
            ClientName = ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint;

            base.Init();            
        }

        public async Task<Subscription> GetSubcription(string subscriptionId, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions/{subscriptionId}?api-version=2020-01-01
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}?api-version=2020-01-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<SubscriptionsDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {                                               
                        var first = ser.Value.FirstOrDefault();                        
                        var result = Mapper.Map<Subscription>(first);
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default)
        {
            try
            {
                //var subId = "33fb38df-688e-4ca1-8dd8-b46e26262ff8";
                //////var data = await _azureSdkService?.TryGetData(subId, null, null);                
                //var sdkSubs = await _azureSdkService?.GetAllSubcriptions(subId, cancelToken);
                //if (sdkSubs?.Count > 0)
                //    return sdkSubs;

                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions?api-version=2020-01-01
                var url = $"https://management.azure.com/subscriptions?api-version=2020-01-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    //var ser = JsonSerializer.Deserialize<SubscriptionsDto>(content, _options);
                    var ser = JsonSerializer.Deserialize<SubscriptionsIdNameDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {
                        //"VM running"
                        //var last = ser.Statuses.Last();
                        var idNameList = ser.Value.Where(a => a.State == "Enabled").Select(a => new IdName(a.SubscriptionId.ToString(), a.DisplayName)).ToList();
                        return idNameList;
                    }
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<List<TagNameWithValueList>> GetTags(string subscriptionId, CancellationToken cancelToken = default)
        {
            try
            {
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions/{subscriptionId}/tagNames?api-version=2020-06-01
                var url = $"https://management.azure.com/subscriptions/{subscriptionId}/tagNames?api-version=2020-06-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var content = await apiData.Content.ReadAsStringAsync();
                    var ser = JsonSerializer.Deserialize<TagsDto>(content, _options);
                    if (ser?.Value?.Count > 0)
                    {
                        var result = new List<TagNameWithValueList>();
                        foreach (var item in ser.Value)
                        {
                            //result.Add(new TagNameWithValueList { TagName = item.TagName, Values = item.Values.Select(a => a.TagValue).ToList()});
                            result.Add(new TagNameWithValueList { TagName = item.TagName.Trim(), Values = item.Values.Where(a => !string.IsNullOrWhiteSpace(a.TagValue)).Select(a => a.TagValue).ToList() });
                        }
                        return result;
                    }
                }
            }
            catch (Exception ex)
            {
                base.HandleException(ex);
            }
            return null;
        }

        public async Task<DefaultTags> GetDefualtTags(string subscriptionId, CancellationToken cancelToken = default)
        {
            var tags = await GetTags(subscriptionId, cancelToken).ConfigureAwait(false);
            if (tags?.Count > 0)
            {
                var result = new DefaultTags();
                foreach(var item in tags)
                {
                    switch(item.TagName)
                    {
                        case nameof(DefaultTag.billingReference):
                            result.BillingReferenceList.AddRange(item.Values);
                            break;
                        case nameof(DefaultTag.cmdbReference):
                            result.CmdbRerenceList.AddRange(item.Values);
                            break;
                        case nameof(DefaultTag.opEnvironment):
                            result.OpEnvironmentList.AddRange(item.Values);
                            break;
                    }
                }
                return result;
            }
            return null;
        }

        public async Task<List<Customer>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            try
            {
                if (subscriptionIds?.Count <= 0)
                    throw new Exception($"missing {nameof(subscriptionIds)} parameter");

                Dictionary<string, Task<DefaultTags>> tasks = new();
                try
                {
                    foreach (var subscriptionId in subscriptionIds)
                    {
                        //var tags = await GetTags(subscriptionId, cancelToken).ConfigureAwait(false);
                        var t = GetDefualtTags(subscriptionId, cancelToken);
                        tasks.Add(subscriptionId, t);
                    }
                    await Task.WhenAll(tasks.Values.ToList());
                }
                catch (Exception ex)
                {
                    throw;
                }

                List<Customer> list = new();
                foreach (var task in tasks)
                {                    
                    Customer customer = new Customer 
                    {
                        SubscriptionId = task.Key
                    };                    
                    if (task.Value.Result.CmdbRerenceList.HasAnyInCollection())
                        task.Value.Result.CmdbRerenceList.ForEach(a => customer.cmdbReferenceList.Add(new cmdbReference { AtCode = a, Email = "N/A" }));
                    if (task.Value.Result.OpEnvironmentList.HasAnyInCollection())
                        task.Value.Result.OpEnvironmentList.ForEach(a => customer.opEnvironmentList.Add(new opEnvironment { Value = a }));

                    list.Add(customer);
                }
                return list;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<Dictionary<string, DefaultTags>> GetDefualtTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {            
            try
            {
                if (subscriptionIds?.Count <= 0)
                    throw new Exception($"missing {nameof(subscriptionIds)} parameter");

                //ConcurrentDictionary<string, DefaultTags> concDic = new();
                //Parallel.ForEach(subscriptionIds, (subscriptionId) =>
                //{
                //    try
                //    {
                //        //var tags = await GetTags(subscriptionId, cancelToken).ConfigureAwait(false);
                //        var t = GetTags(subscriptionId, cancelToken);
                //        t.Wait();
                //        var tags = t.Result;
                //        if (tags?.Count > 0)
                //        {
                //            var result = new DefaultTags();
                //            foreach (var item in tags)
                //            {
                //                switch (item.TagName)
                //                {
                //                    case nameof(DefaultTag.billingReference):
                //                        result.BillingReferenceList.AddRange(item.Values);
                //                        break;
                //                    case nameof(DefaultTag.cmdbReference):
                //                        result.CmdbRerenceList.AddRange(item.Values);
                //                        break;
                //                    case nameof(DefaultTag.opEnvironment):
                //                        result.OpEnvironmentList.AddRange(item.Values);
                //                        break;
                //                }
                //            }
                //            if (tags?.Count > 0)
                //                //concDic.TryAdd(subscriptionId, result);
                //                concDic.AddOrUpdate(subscriptionId, result, (key, oldValue) => result);
                //        }
                //    }
                //    catch (Exception ex)
                //    {
                //        throw;
                //    }
                //});
                //Dictionary<string, DefaultTags> d = concDic.ToDictionary(pair => pair.Key, pair => pair.Value);
                //return d;

                Dictionary<string, Task<DefaultTags>> tasks = new();
                try
                {
                    foreach (var subscriptionId in subscriptionIds)
                    {
                        //var tags = await GetTags(subscriptionId, cancelToken).ConfigureAwait(false);
                        var t = GetDefualtTags(subscriptionId, cancelToken);                        
                        tasks.Add(subscriptionId, t);
                    }
                    await Task.WhenAll(tasks.Values.ToList());
                }
                catch (Exception ex)
                {
                    throw;
                }

                Dictionary<string, DefaultTags> d = new();
                foreach(var task in tasks)
                {
                    d.Add(task.Key, task.Value.Result);
                }
                return d;
            }
            catch (Exception)
            {
                throw;
            }
        }

        public async Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default)
        {
            var res = await GetSubcription(subscriptionId, cancelToken);
            return res != null;
        }
    }

}
