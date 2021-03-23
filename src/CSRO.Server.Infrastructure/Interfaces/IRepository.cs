using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure
{
    public interface IRepository<TModel> where TModel : class
    {
        DbContext DatabaseContext { get; }                

        Task<TModel> GetId(int id);        

        Task<List<TModel>> GetList();
        void Add(TModel entity, string UserId = null);
        void Remove(TModel entity, string UserId = null);
        void Update(TModel entity, string UserId = null);
        Task<TModel> UpdateAsync(TModel entity, string UserId = null);
        Task<bool> SaveChangesAsync();
        Task<TModel> GetFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes);
        Task<TModel> GetFilter(Expression<Func<TModel, bool>> expression);
        Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression);
        Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes);        
    }
}
