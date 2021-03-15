using System;
using System.Collections.Generic;
using System.Text;
using CSRO.Server.Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSRO.Server.Entities
{
    public class AdoContext : DbContext
    {
        private readonly string _connectionString;

        public AdoContext() : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public AdoContext(string connectionString) : base()
        {
            _connectionString = connectionString;

            //exception here
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public AdoContext(DbContextOptions<AdoContext> options)
           : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public DbSet<AdoProject> AdoProjects { get; set; }

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
                //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
            }
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //modelBuilder.Entity<AppVersion>().Property(b => b.Id).HasIdentityOptions(startValue: 22);
            //modelBuilder.Entity<MessageSection>().Property(b => b.MessageSectionId).HasIdentityOptions(startValue: 2000);

            //modelBuilder.Entity<MessageDetail>().Property(b => b.MessageDetailId).HasIdentityOptions(startValue: 100);            
            //modelBuilder.Entity<Message>().HasKey(a => a.MessageId);            
            //modelBuilder.UseSerialColumns();
            //modelBuilder.Entity<Message>().Property(b => b.MessageId).UseIdentityAlwaysColumn();

            //modelBuilder.Entity<AppVersion>()
            //   .Property(a => a.RowVersion)
            //   .IsRowVersion();
            //modelBuilder.Entity<Ticket>()
            //   .Property(a => a.RowVersion)
            //   .IsRowVersion();

            modelBuilder.Entity<AdoProject>().HasData(
                new AdoProject()
                {
                    Id = 1,
                    CreatedBy = "Migration Script",
                    CreatedAt = new DateTime(2021, 11, 1, 14, 15, 16),
                    Name = "Dymmy record", 
                    ProcessName = "Agile", 
                    Description = "Fake Not created", 
                    Organization = "SomeOrg", 
                    State = ProjectState.Deleted,                    
                }

                //,
                //new AppVersion()
                //{
                //    Id = 21,
                //    VersionValue = 21,
                //    VersionFull = "1.0.21.0",
                //    CreatedAt = new DateTime(2020, 11, 25, 11, 0, 0),
                //    ReleasedAt = new DateTime(2020, 11, 25, 17, 0, 0),
                //    Details = "This is version 21, modified at 5:00 pm",
                //    DetailsFormat = "text",
                //    Link = "www.google.sk",
                //    RecomendedAction = RecomendedAction.Warning,
                //}
                );
        }

    }
}
