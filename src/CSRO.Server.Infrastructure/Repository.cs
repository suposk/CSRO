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

        public Repository(DbContext context)
        {
            this.DatabaseContext = context;
        }

        public virtual void Add(TModel entity, string UserId = null)
        {
            DatabaseContext.Entry(entity).State = EntityState.Added;
            DatabaseContext.Set<TModel>().Add(entity);
            entity.CreatedBy = UserId;
            entity.CreatedAt = DateTime.UtcNow;
        }
        public virtual void Update(TModel entity, string UserId = null)
        {
            DatabaseContext.Entry(entity).State = EntityState.Modified;
            entity.ModifiedBy = UserId;
            entity.ModifiedAt = DateTime.UtcNow;
        }

        public virtual void Remove(TModel entity, string UserId = null)
        {
            DatabaseContext.Entry(entity).State = EntityState.Deleted;
            DatabaseContext.Set<TModel>().Remove(entity);
            entity.ModifiedBy = UserId;
            entity.ModifiedAt = DateTime.UtcNow;
        }

        public virtual Task<TModel> GetByFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes)
        {
            DbSet<TModel> dbSet = DatabaseContext.Set<TModel>();
            IQueryable<TModel> query = null;
            foreach (var includeExpression in includes)
            {
                query = dbSet.Include(includeExpression);
            }
            return query.FirstOrDefaultAsync(expression);
        }

        public virtual Task<TModel> GetByFilter(Expression<Func<TModel, bool>> expression)
        {
            return DatabaseContext.Set<TModel>().FirstOrDefaultAsync(expression);
        }

        public virtual Task<List<TModel>> GetAllAsync()
        {
            return DatabaseContext.Set<TModel>().ToListAsync();
        }

        public virtual async Task<TModel> GetAsync(int id)
        {
            return await DatabaseContext.Set<TModel>().FindAsync(id);
        }

        public virtual async Task<bool> SaveChangesAsync()
        {
            return await DatabaseContext.SaveChangesAsync() >= 0;
        }


    }
}
