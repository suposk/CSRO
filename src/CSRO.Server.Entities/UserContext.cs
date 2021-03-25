﻿using System;
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
        public DbSet<AdoProject> AdoProjects { get; set; }
        public DbSet<AdoProjectHistory> AdoProjectHistorys { get; set; }

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
            const string firstUser = "13229d33-99e0-41b3-b18d-4f72127e3971";
            const string secondUser = "96053525-f4a5-47ee-855e-0ea77fa6c55a";

            modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

            modelBuilder.Entity<User>().HasData(
                new User()
                {
                    Id = 1,
                    GuidId = new Guid(firstUser),                                       
                    Username = "jan.supolik_hotmail.com#EXT#@jansupolikhotmail.onmicrosoft.com",
                    Active = true
                },
                new User()
                {
                    Id = 2,
                    GuidId = new Guid(secondUser),
                    Username = "read@jansupolikhotmail.onmicrosoft.com",
                    Active = true
                });

            modelBuilder.Entity<UserClaim>().HasData(
                //first user
             new UserClaim()
             {
                 Id = 1,
                 UserId = 1,
                 UserGuidId = new Guid(firstUser),
                 Type = ClaimTypesCsro.CanApproveAdoRequest,
                 Value = "Frank"
             },
             new UserClaim()
             {
                 Id = 2,
                 UserId = 1,
                 UserGuidId = new Guid(firstUser),
                 Type = ClaimTypes.Email,
                 Value = "jan.supolik@hotmail.com"
             },
             //second user
             //new UserClaim()
             //{
             //    Id = 20,
             //    UserId = 2,
             //    UserGuidId = new Guid(secondUser),
             //    Type = "given_name",
             //    Value = "Claire"
             //},
             new UserClaim()
             {
                 Id = 21,
                 UserId = 2,
                 UserGuidId = new Guid(secondUser),
                 Type = ClaimTypes.Email,
                 Value = "fake@someprovider.com"
             }
             );
        }

    }
}
