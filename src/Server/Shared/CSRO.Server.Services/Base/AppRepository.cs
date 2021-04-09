using CSRO.Server.Entities;
using CSRO.Server.Infrastructure;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Services.Base
{
    public class AppRepository<TModel> : Repository<TModel>, IRepository<TModel> where TModel : EntityBase
    {
        public AppRepository(AppVersionContext context, IApiIdentity apiIdentity) : base(context, apiIdentity)
        {

        }
    }
}
