using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Text;
using CSRO.Server.Entities.Entity;
using CSRO.Server.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CSRO.Server.Entities
{
    public class UserContext : DbContext
    {
        private readonly string _connectionString;

        public UserContext() : base()
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public UserContext(string connectionString) : base()
        {
            _connectionString = connectionString;

            //exception here
            //ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }

        public UserContext(DbContextOptions<UserContext> options)
           : base(options)
        {
            ChangeTracker.QueryTrackingBehavior = QueryTrackingBehavior.NoTracking;
        }
        public DbSet<User> Users { get; set; }

        public DbSet<UserClaim> UserClaims { get; set; }

        //public DbSet<UserLogin> UserLogins { get; set; }
        

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
            const string firstUser = "8aa6a8cb-36ed-415a-a12b-07c84af45428";
            const string secondUser = "44769cb1-cca7-4a19-8bbe-8edea9b99179";

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    ObjectId = new Guid(firstUser),                                       
                    Username = "live.com#jan.supolik@hotmail.com",
                    Active = true
                },
                new User()
                {
                    Id = 2,
                    ObjectId = new Guid(secondUser),
                    Username = "read@jansupolikhotmail.onmicrosoft.com",
                    Active = true
                });

            modelBuilder.Entity<UserClaim>().HasData(
             //first user
             new UserClaim()
             {
                 Id = 1,
                 UserId = 1,
                 //UserGuidId = new Guid(firstUser),
                 Type = ClaimTypesCsro.CanApproveAdoRequest,
                 Value = "True"
             },
             new UserClaim()
             {
                 Id = 2,
                 UserId = 1,                 
                 Type = ClaimTypesCsro.CanReadAdoRequest,
                 Value = "True"
             },
             new UserClaim()
             {
                 Id = 3,
                 UserId = 1,                 
                 Type = ClaimTypes.Email,
                 Value = "jan.supolik@hotmail.com"
             },

             //second user
             new UserClaim()
             {
                 Id = 21,
                 UserId = 1,                 
                 Type = ClaimTypesCsro.CanReadAdoRequest,
                 Value = "True"
             },
             new UserClaim()
             {
                 Id = 22,
                 UserId = 2,                 
                 Type = ClaimTypes.Email,
                 Value = "fake@someprovider.com"
             }
             );
        }

    }
}
