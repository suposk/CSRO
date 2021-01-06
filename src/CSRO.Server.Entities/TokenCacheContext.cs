using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using CSRO.Server.Entities.Entity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CSRO.Server.Entities
{
    [DbContext(typeof(TokenCacheContext))]
    [Migration("RunSqlScripts")] // Change 'RunSqlScripts' to your migration name
    public class RunSqlScripts : Migration // Change 'RunSqlScripts' to your migration name
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // Change filename to the name of your custom sql migration file
            //migrationBuilder.Sql(File.ReadAllText("SqlScripts/CreateCache.sql"));
            var path = Path.Combine(Directory.GetCurrentDirectory(), "SqlScripts\\CreateCache.sql");                       
            migrationBuilder.Sql(File.ReadAllText(path));            
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // custom down code here
        }
    }

    public class TokenCacheContext : DbContext
    {
        private readonly string _connectionString;

        public TokenCacheContext() : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public TokenCacheContext(string connectionString) : base()
        {
            _connectionString = connectionString;

            //exception here
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public TokenCacheContext(DbContextOptions<TokenCacheContext> options)
           : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public DbSet<AppVersion> AppVersions { get; set; }
        public DbSet<Ticket> Tickets { get; set; }

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
        }

    }
}
