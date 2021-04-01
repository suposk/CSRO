//using CSRO.Common.AdoServices.Models;
using CSRO.Server.Entities;
using CSRO.Server.Infrastructure;
//using MediatR;
//using CSRO.Server.Ado.Api.Commands;

namespace CSRO.Server.Ado.Api.Services
{
    public class AdoRepository<TModel> : Repository<TModel>, IRepository<TModel> where TModel : EntityBase
    {
        public AdoRepository(AdoContext context, IApiIdentity apiIdentity) : base(context, apiIdentity)
        {

        }
    }
}
