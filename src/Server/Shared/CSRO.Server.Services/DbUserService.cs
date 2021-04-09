using CSRO.Server.Entities;
using CSRO.Server.Entities.Entity;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace CSRO.Server.Services
{
    public class DbUserService : ILocalUserService
    {
        private readonly UserContext _context;        

        public DbUserService(
            UserContext context
            //IPasswordHasher<User> passwordHasher
            )
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }


        //public async Task<bool> IsUserActive(string subject)
        //{
        //    if (string.IsNullOrWhiteSpace(subject))
        //    {
        //        return false;
        //    }

        //    var user = await GetUserBySubjectAsync(subject);

        //    if (user == null)
        //    {
        //        return false;
        //    }

        //    return user.Active;
        //}


        public async Task<User> GetUserByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))            
                throw new ArgumentNullException(nameof(userName));            

            return await _context.Users
                 .FirstOrDefaultAsync(u => u.Username == userName);
        }

        public async Task<List<UserClaim>> GetUserClaimsByUserNameAsync(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))            
                throw new ArgumentNullException(nameof(userName));            

            return await _context.UserClaims.Where(u => u.User.Username == userName).ToListAsync();
        }

        public async Task<List<Claim>> GetClaimsByUserNameAsync(string userName)
        {
            var userDbClaims = await GetUserClaimsByUserNameAsync(userName);
            if (userDbClaims.HasAnyInCollection())
            {
                List<Claim> list = new();
                userDbClaims.ForEach(a => list.Add(new Claim(a.Type, a.Value)));
                return list;
            }
            else return null;
        }

        public void AddUser(User userToAdd, string password)
        {
            if (userToAdd == null)
            {
                throw new ArgumentNullException(nameof(userToAdd));
            }

            //if (string.IsNullOrWhiteSpace(password))
            //{
            //    throw new ArgumentNullException(nameof(password));
            //}

            if (_context.Users.Any(u => u.Username == userToAdd.Username))
            {
                // in a real-life scenario you'll probably want to 
                // return this as a validation issue
                throw new Exception("Username must be unique");
            }

            //if (_context.Users.Any(u => u.Email == userToAdd.Email))
            //{
            //    // in a real-life scenario you'll probably want to 
            //    // return this a a validation issue
            //    throw new Exception("Email must be unique");
            //}

            //using (var randomNumberGenerator = new RNGCryptoServiceProvider())
            //{
            //    var securityCodeData = new byte[128];
            //    randomNumberGenerator.GetBytes(securityCodeData);
            //    userToAdd.SecurityCode = Convert.ToBase64String(securityCodeData);
            //}

            //userToAdd.SecurityCodeExpirationDate = DateTime.UtcNow.AddHours(1);

            //userToAdd.Password = _passwordHasher.HashPassword(userToAdd, password);
            _context.Users.Add(userToAdd);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return (await _context.SaveChangesAsync() > 0);
        }
    }
}
