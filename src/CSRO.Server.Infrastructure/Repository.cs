using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace CSRO.Server.Infrastructure
{
    public class Repository<TModel> : IRepository<TModel> where TModel : class
    {
        public DbContext DatabaseContext { get; private set; }

        public Repository(DbContext context)
        {
            this.DatabaseContext = context;
        }

        public void Add(TModel entity)
        {
            DatabaseContext.Set<TModel>().Add(entity);
        }

        public TModel Get(int id)
        {
            return DatabaseContext.Set<TModel>().Find(id);
        }

        public Task<TModel> GetByFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes)
        {
            DbSet<TModel> dbSet = DatabaseContext.Set<TModel>();
            IQueryable<TModel> query = null;
            foreach (var includeExpression in includes)
            {
                query = dbSet.Include(includeExpression);
            }
            return query.FirstOrDefaultAsync(expression);
        }

        public Task<TModel> GetByFilter(Expression<Func<TModel, bool>> expression)
        {
            return DatabaseContext.Set<TModel>().FirstOrDefaultAsync(expression);
        }

        public List<TModel> GetAll()
        {
            return DatabaseContext.Set<TModel>().ToList();
        }

        public Task<List<TModel>> GetAllAsync()
        {
            return DatabaseContext.Set<TModel>().ToListAsync();
        }

        public async Task<TModel> GetAsync(int id)
        {
            return await DatabaseContext.Set<TModel>().FindAsync(id);
        }

        public void Remove(TModel entity)
        {
            DatabaseContext.Entry(entity).State = EntityState.Deleted;
            DatabaseContext.Set<TModel>().Remove(entity);
        }

        public async Task<bool> SaveChangesAsync()
        {
            return await DatabaseContext.SaveChangesAsync() >= 0;
        }

        public void UpdateGeneric<T>(T entity) where T : class
        {
            DatabaseContext.Entry(entity).State = EntityState.Modified;
        }
    }
}
