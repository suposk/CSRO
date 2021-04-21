﻿using CSRO.Server.Infrastructure;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Entities;
using System.Threading.Tasks;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace CSRO.Server.Services
{
    public class CustomerRepository : ICustomerRepository
    {
        private readonly IApiIdentity _apiIdentity;
        public CustomersDbContext Context { get; }
        public CustomerRepository
            (
            CustomersDbContext context, 
            IApiIdentity apiIdentity
            )
        {
            Context = context;
            _apiIdentity = apiIdentity;
        }        

        public Task<List<ResourceSWI>> GetCustomersBySubNames(List<string> subscriptionNames)
        {                        
            var q = Context.ResourceSWIs.Where(a => subscriptionNames.Contains(a.SubscriptionName)).ToListAsync();                 
            return q;
        }

        public Task<List<ResourceSWI>> GetCustomersBySubName(string subscriptionName)
        {
            var q = Context.ResourceSWIs.Where(a => a.SubscriptionName.Contains(subscriptionName)).ToListAsync();
            return q;
        }

        public Task<List<ResourceSWI>> GetCustomersBySubIds(List<string> subscriptionIds)
        {
            var q = Context.ResourceSWIs.Where(a => subscriptionIds.Contains(a.SubscriptionId)).ToListAsync();
            return q;
        }

        public Task<List<ResourceSWI>> GetCustomersBySubId(string subscriptionId)
        {
            var q = Context.ResourceSWIs.Where(a => a.SubscriptionId == subscriptionId).ToListAsync();
            return q;
        }

        public Task<List<ResourceSWI>> GetCustomersByRegions(List<string> regions)
        {
            var q = Context.ResourceSWIs.Where(a => regions.Contains(a.ResourceLocation)).ToListAsync();
            return q;
        }

        public Task<List<ResourceSWI>> GetCustomersByRegion(string region)
        {
            var q = Context.ResourceSWIs.Where(a => a.ResourceLocation.Contains(region)).ToListAsync();
            return q;
        }

        public Task<List<ResourceSWI>> GetCustomersByAtCodes(List<string> atCodes)
        {
            var q = Context.ResourceSWIs.Where(a => atCodes.Contains(a.AtCode)).ToListAsync();
            return q;
        }

        public Task<List<ResourceSWI>> GetCustomersByAtCode(string atCode)
        {
            var q = Context.ResourceSWIs.Where(a => a.AtCode == atCode).ToListAsync();
            return q;
        }
    }
}
