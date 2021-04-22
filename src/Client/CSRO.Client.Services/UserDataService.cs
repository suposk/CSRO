using AutoMapper;
using CSRO.Client.Core;
using CSRO.Client.Services.Dtos;
using CSRO.Client.Services.Models;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CSRO.Client.Services
{

    public interface IUserDataService : IBaseDataService<User>
    {
        Task<User> GetUserByUserName(string userName);
    }

    public class UserDataService : BaseDataService, IUserDataService
    {

        public UserDataService(
            IHttpClientFactory httpClientFactory,
            IAuthCsroService authCsroService,
            IMapper mapper,
            IConfiguration configuration)
            : base(httpClientFactory, authCsroService, mapper, configuration)
        {
            ApiPart = "api/user/";
            Scope = Configuration.GetValue<string>(ConstatCsro.Scopes.Scope_Auth_Api);
            ClientName = ConstatCsro.EndPoints.ApiEndpointAuth;

            base.Init();
        }

        public Task<User> AddItemAsync(User item)
        {
            return base.RestAdd<User, UserDto>(item);
        }

        public Task<bool> UpdateItemAsync(User item)
        {
            return base.RestUpdate<User, UserDto>(item);
        }

        public Task<bool> DeleteItemAsync(int id)
        {
            return base.RestDeleteById(id);
        }

        public Task<User> GetItemByIdAsync(int id)
        {
            return base.RestGetById<User, UserDto>(id.ToString());
        }

        public Task<List<User>> GetItemsAsync()
        {
            return base.RestGetListById<User, UserDto>();
        }

        public Task<User> GetUserByUserName(string userName)
        {
            return base.RestGetById<User, UserDto>(userName);
        }

    }

}
