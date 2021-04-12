//using CSRO.Common.AdoServices.Models;
using CSRO.Server.Entities;
using CSRO.Server.Infrastructure;
//using MediatR;
//using CSRO.Server.Ado.Api.Commands;

namespace CSRO.Server.Auth.Api.Services
{
    public class AuthRepository<TModel> : Repository<TModel>, IRepository<TModel> where TModel : EntityBase
    {
        public AuthRepository(UserContext context, IApiIdentity apiIdentity) : base(context, apiIdentity)
        {

        }
    }
}
