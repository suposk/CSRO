using System;
using CSRO.Server.Entities.Entity;
using Microsoft.EntityFrameworkCore;

namespace CSRO.Server.Entities
{
    public class CustomersDbContext : DbContext
    {
        private readonly string _connectionString;

        public CustomersDbContext() : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public CustomersDbContext(string connectionString) : base()
        {
            _connectionString = connectionString;
        }

        public CustomersDbContext(DbContextOptions<CustomersDbContext> options)
           : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<ResourceSWI> ResourceSWIs { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                //sql Lite
                if (!string.IsNullOrWhiteSpace(_connectionString))
                {
                    //optionsBuilder.UseSqlite(_connectionString);
                    optionsBuilder.UseSqlServer(_connectionString);
                }                
            }
        }


        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<ResourceSWI>().HasNoKey();
            //modelBuilder.Entity<ResourceSWI>().HasData(
            //    new ResourceSWI()
            //    {
            //        AtCode = "AT25813",
            //        AtName = "AA12345",
            //        AtSwc = "Some SWC Name",
            //        Email = "jozo.mrkvicka@bla.com",
            //        EmailGroup = "ABC_Group",
            //        SubscriptionId = "33fb38df-688e-4ca1-8dd8-b46e26262ff8",
            //        SubscriptionName = "Js-Pay-As-You-Go",
            //        ResourceGroup = "csro-rg-neu-dev",
            //        AzureResource  = "rg", 
            //        ResourceLocation = "North Europe",
            //        OpEnvironment = "DEV",
            //        ResourceType = "Microsoft.Resources/deployments"
            //    },
            //    new ResourceSWI()
            //    {
            //        AtCode = "AT25818",
            //        AtName = "AA12345",
            //        AtSwc = "Some SWC Name",
            //        Email = "ferko.mrkvicka@bla.com",
            //        EmailGroup = null,
            //        SubscriptionId = "33fb38df-688e-4ca1-8dd8-b46e26262ff8",
            //        SubscriptionName = "Js-Pay-As-You-Go",
            //        ResourceGroup = "rg-prod-weu",
            //        AzureResource = "rg",
            //        ResourceLocation = "West Europe",
            //        OpEnvironment = "PROD",
            //        ResourceType = "Microsoft.Resources/deployments"
            //    },
            //    new ResourceSWI()
            //    {
            //        AtCode = "AT25815",
            //        AtName = "AA12345",
            //        AtSwc = "Some SWC Name",
            //        Email = null,
            //        EmailGroup = "ABC_Group",
            //        SubscriptionId = "634e6b93-264e-44f0-9e87-3606169fee2f",
            //        SubscriptionName = "dev-Pay-As-You-Go2",
            //        ResourceGroup = "neu-rg-2nd",
            //        AzureResource = "rg",
            //        ResourceLocation = "North Europe",
            //        OpEnvironment = "DEV",
            //        ResourceType = "Microsoft.Resources/deployments"
            //    })
            //    ;
        }

    }
}
