using AutoMapper;
using CSRO.Server.Domain;
using CSRO.Server.Domain.AzureDtos;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Services.Models;
using Microsoft.Extensions.Configuration;
using Microsoft.Identity.Web;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;

namespace CSRO.Server.Services.AzureRestServices
{
    public interface ISubcriptionService
    {
        Task<bool> SubcriptionExist(string subscriptionId, CancellationToken cancelToken = default);
        Task<List<IdName>> GetSubcriptions(CancellationToken cancelToken = default);
        Task<Subscription> GetSubcription(string subscriptionId, CancellationToken cancelToken = default);
        Task<List<TagNameWithValueListModel>> GetTags(string subscriptionId, CancellationToken cancelToken = default);
        Task<DefaultTagsModel> GetDefualtTags(string subscriptionId, CancellationToken cancelToken = default);
        Task<Dictionary<string, DefaultTagsModel>> GetDefualtTags(List<string> subscriptionIds, CancellationToken cancelToken = default);
        Task<List<CustomerModel>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default);
    }

    public class SubcriptionService : BaseDataService, ISubcriptionService
    {

        public SubcriptionService(
            IHttpClientFactory httpClientFactory,
            //IAuthCsroService authCsroService,
            ITokenAcquisition tokenAcquisition,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, tokenAcquisition, configuration)
        {
            Mapper = mapper;

            ApiPart = "--";            
            Scope = Core.ConstatCsro.Scopes.MANAGEMENT_AZURE_SCOPE;            
            ClientName = Core.ConstatCsro.ClientNames.MANAGEMENT_AZURE_EndPoint;

            base.Init();
            
        }

        public IMapper Mapper { get; }

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
                //1. Call azure api
                await base.AddAuthHeaderAsync();

                //GET https://management.azure.com/subscriptions?api-version=2020-01-01
                var url = $"https://management.azure.com/subscriptions?api-version=2020-01-01";
                var apiData = await HttpClientBase.GetAsync(url, cancelToken).ConfigureAwait(false);

                if (apiData.IsSuccessStatusCode)
                {
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<SubscriptionsIdNameDto>(stream, _options);
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

        public async Task<List<TagNameWithValueListModel>> GetTags(string subscriptionId, CancellationToken cancelToken = default)
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
                    var stream = await apiData.Content.ReadAsStreamAsync();
                    var ser = await JsonSerializer.DeserializeAsync<TagsDto>(stream, _options);
                    if (ser?.Value?.Count > 0)
                    {
                        var result = new List<TagNameWithValueListModel>();
                        foreach (var item in ser.Value)
                        {
                            //result.Add(new TagNameWithValueList { TagName = item.TagName, Values = item.Values.Select(a => a.TagValue).ToList()});
                            result.Add(new TagNameWithValueListModel { TagName = item.TagName.Trim(), Values = item.Values.Where(a => !string.IsNullOrWhiteSpace(a.TagValue)).Select(a => a.TagValue).ToList() });
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

        public async Task<DefaultTagsModel> GetDefualtTags(string subscriptionId, CancellationToken cancelToken = default)
        {
            var tags = await GetTags(subscriptionId, cancelToken).ConfigureAwait(false);
            if (tags?.Count > 0)
            {
                var result = new DefaultTagsModel();
                foreach (var item in tags)
                {
                    switch (item.TagName)
                    {
                        case nameof(DefaultTagModel.billingReference):
                            result.BillingReferenceList.AddRange(item.Values);
                            break;
                        case nameof(DefaultTagModel.cmdbReference):
                            result.CmdbRerenceList.AddRange(item.Values);
                            break;
                        case nameof(DefaultTagModel.opEnvironment):
                            result.OpEnvironmentList.AddRange(item.Values);
                            break;
                    }
                }
                return result;
            }
            return null;
        }

        public Task<List<CustomerModel>> GetTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            try
            {
                if (subscriptionIds?.Count <= 0)
                    throw new Exception($"missing {nameof(subscriptionIds)} parameter");

                #region Task.WhenAll
                //Dictionary<string, Task<DefaultTags>> tasks = new();
                //try
                //{
                //    foreach (var subscriptionId in subscriptionIds)
                //    {
                //        //var tags = await GetTags(subscriptionId, cancelToken).ConfigureAwait(false);
                //        var t = GetDefualtTags(subscriptionId, cancelToken);
                //        tasks.Add(subscriptionId, t);
                //    }
                //    await Task.WhenAll(tasks.Values.ToList());
                //}
                //catch (Exception ex)
                //{
                //    throw;
                //}

                //List<Customer> list = new();
                //foreach (var task in tasks)
                //{
                //    Customer customer = new Customer
                //    {
                //        SubscriptionId = task.Key
                //    };
                //    if (task.Value.Result.CmdbRerenceList.HasAnyInCollection())
                //        task.Value.Result.CmdbRerenceList.ForEach(a => customer.cmdbReferenceList.Add(new cmdbReference { AtCode = a, Email = "N/A" }));
                //    if (task.Value.Result.OpEnvironmentList.HasAnyInCollection())
                //        task.Value.Result.OpEnvironmentList.ForEach(a => customer.opEnvironmentList.Add(new opEnvironment { Value = a }));

                //    list.Add(customer);
                //}
                //return list;
                #endregion

                ConcurrentDictionary<string, DefaultTagsModel> concDic = new();
                Parallel.ForEach(subscriptionIds, (subscriptionId) =>
                {
                    try
                    {                        
                        var t = GetDefualtTags(subscriptionId, cancelToken);
                        t.Wait();
                        var result = t.Result;
                        if (result != null)
                        {                            
                            //concDic.TryAdd(subscriptionId, result);
                            concDic.AddOrUpdate(subscriptionId, result, (key, oldValue) => result);
                        }
                    }
                    catch (Exception ex)
                    {
                        throw;
                    }
                });
                if (concDic?.Count == 0)
                    return null;

                List<CustomerModel> list = new();
                foreach(var pair in concDic)
                {
                    CustomerModel customer = new CustomerModel
                    {
                        SubscriptionId = pair.Key
                    };
                    if (pair.Value.CmdbRerenceList.HasAnyInCollection())
                        //pair.Value.CmdbRerenceList.ForEach(a => customer.cmdbReferenceList.Add(new cmdbReferenceModel { AtCode = a, Email = "N/A" }));
                        pair.Value.CmdbRerenceList.ForEach(a => customer.cmdbReferenceList.Add(new cmdbReferenceModel { AtCode = a }));
                    if (pair.Value.OpEnvironmentList.HasAnyInCollection())
                        pair.Value.OpEnvironmentList.ForEach(a => customer.opEnvironmentList.Add(new opEnvironmentModel { Value = a }));

                    list.Add(customer);
                }
                return Task.FromResult(list);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<Dictionary<string, DefaultTagsModel>> GetDefualtTags(List<string> subscriptionIds, CancellationToken cancelToken = default)
        {
            try
            {
                if (subscriptionIds?.Count <= 0)
                    throw new Exception($"missing {nameof(subscriptionIds)} parameter");

                #region Parallel.ForEach

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
                #endregion

                Dictionary<string, Task<DefaultTagsModel>> tasks = new();
                try
                {
                    foreach (var subscriptionId in subscriptionIds)
                    {
                        var t = GetDefualtTags(subscriptionId, cancelToken);
                        tasks.Add(subscriptionId, t);
                    }
                    await Task.WhenAll(tasks.Values.ToList());
                }
                catch (Exception ex)
                {
                    throw;
                }

                Dictionary<string, DefaultTagsModel> d = new();
                foreach (var task in tasks)
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
