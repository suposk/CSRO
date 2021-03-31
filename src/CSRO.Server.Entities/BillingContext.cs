using System;
using CSRO.Server.Entities.Entity;
using Microsoft.EntityFrameworkCore;

namespace CSRO.Server.Entities
{
    public class BillingContext : DbContext
    {
        private readonly string _connectionString;

        public BillingContext() : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public BillingContext(string connectionString) : base()
        {
            _connectionString = connectionString;

            //exception here
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public BillingContext(DbContextOptions<BillingContext> options)
           : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<AtCodecmdbReference> AtCodecmdbReferences { get; set; }

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
            modelBuilder.Entity<AtCodecmdbReference>().HasData(
                new AtCodecmdbReference()
                {
                    Id = 1,                                        
                    AtCode = "AT25813",
                    Email = "jozo.mrkvicka@bla.com",
                    CreatedAt = new DateTime(2020, 11, 25, 11, 0, 0),
                    CreatedBy = "Mig Script",
                },
                new AtCodecmdbReference()
                {
                    Id = 2,
                    AtCode = "AT25815",
                    Email = "ferko.mrkvicka@bla.com",
                    CreatedAt = new DateTime(2020, 12, 28, 11, 0, 0),
                    CreatedBy = "Mig Script",
                })
                ;
        }

    }
}
