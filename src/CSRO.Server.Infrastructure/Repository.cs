using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure
{
    public class Repository<TModel> : IRepository<TModel> where TModel : EntityBase
    {
        public DbContext DatabaseContext { get; private set; }
        public IApiIdentity ApiIdentity { get; }

        public Repository(DbContext context, IApiIdentity apiIdentity)
        {
            DatabaseContext = context;
            ApiIdentity = apiIdentity;
        }

        public virtual void Add(TModel entity, string UserId = null)
        {
            entity.CreatedBy = UserId ?? ApiIdentity.GetUserName();
            entity.CreatedAt = DateTime.UtcNow;
            //entity.ModifiedAt = DateTime.UtcNow;
            //entity.ModifiedBy = UserId;            
            DatabaseContext.Set<TModel>().Add(entity);
        }
        public virtual void Update(TModel entity, string UserId = null)
        {
            DatabaseContext.Entry(entity).State = EntityState.Modified;
            entity.ModifiedBy = UserId ?? ApiIdentity.GetUserName();
            entity.ModifiedAt = DateTime.UtcNow;
        }

        public virtual void Remove(TModel entity, string UserId = null)
        {
            if (entity is EntitySoftDeleteBase)
            {
                (entity as EntitySoftDeleteBase).IsDeleted = true;
                Update(entity, UserId ?? ApiIdentity.GetUserName());
            }
            else
            {
                DatabaseContext.Entry(entity).State = EntityState.Deleted;
                DatabaseContext.Set<TModel>().Remove(entity);
                //DatabaseContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual Task<TModel> GetFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes)
        {
            DbSet<TModel> dbSet = DatabaseContext.Set<TModel>();
            IQueryable<TModel> query = null;
            foreach (var includeExpression in includes)
            {
                query = dbSet.Include(includeExpression);
            }
            return query.FirstOrDefaultAsync(expression);
        }

        public virtual Task<TModel> GetFilter(Expression<Func<TModel, bool>> expression)
        {
            return DatabaseContext.Set<TModel>().FirstOrDefaultAsync(expression);
        }

        public virtual Task<List<TModel>> GetList()
        {
            return DatabaseContext.Set<TModel>().ToListAsync();
        }

        public virtual async Task<TModel> GetId(int id)
        {
            return await DatabaseContext.Set<TModel>().FindAsync(id);
        }

        public virtual Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression)
        {
            return DatabaseContext.Set<TModel>().Where(expression).ToListAsync();
        }

        public virtual Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes)
        {
            DbSet<TModel> dbSet = DatabaseContext.Set<TModel>();
            IQueryable<TModel> query = null;
            foreach (var includeExpression in includes)
            {
                query = dbSet.Include(includeExpression);
            }

            return query.Where(expression).ToListAsync();
        }

        public virtual async Task<bool> SaveChangesAsync()
        {
            return await DatabaseContext.SaveChangesAsync() >= 0;
        }


    }
}
