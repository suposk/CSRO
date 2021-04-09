using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
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

        public virtual async Task<TModel> UpdateAsync(TModel entity, string UserId = null)
        {
            DatabaseContext.Entry(entity).State = EntityState.Modified;
            entity.ModifiedBy = UserId ?? ApiIdentity.GetUserName();
            entity.ModifiedAt = DateTime.UtcNow;
            await SaveChangesAsync();
            return entity;
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
            return DatabaseContext.Set<TModel>().OrderByDescending(a => a.CreatedAt).ToListAsync();
        }

        public virtual async Task<TModel> GetId(int id)
        {
            return await DatabaseContext.Set<TModel>().FindAsync(id).ConfigureAwait(false);
        }

        public virtual Task<List<TModel>> GetByUserId(string userId)
        {
            //var softDelete = entity as EntitySoftDeleteBase;
            //if (softDelete != null)
            //    //return DatabaseContext.Set<TModel>().Where(a => softDelete.IsDeleted != true && softDelete.CreatedBy.Contains(userId)).ToListAsync();
            //    return DatabaseContext.Set<TModel>().Where(a => a.CreatedBy.Contains(userId)).ToListAsync();

            //var b = entity as EntityBase;
            //if (b != null)
                return DatabaseContext.Set<TModel>().Where(a => a.CreatedBy.Contains(userId)).OrderByDescending(a => a.CreatedAt).ToListAsync();
            //return null;
        }

        public virtual Task<List<TModel>> GetListFilter(Expression<Func<TModel, bool>> expression)
        {
            return DatabaseContext.Set<TModel>().Where(expression).OrderByDescending(a => a.CreatedAt).ToListAsync();
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
            try
            {
                var saved = await DatabaseContext.SaveChangesAsync().ConfigureAwait(false) >= 0;
                return saved;
                // move on
            }
            catch (DbUpdateException e)
            {
                // get latest version of record for display
                throw new DbUpdateException("Conflict detected, refresh and try again.");
            }
            catch (Exception ee)
            {
                return false;
            }
        }

        #region old Save
        //public virtual async Task<bool> SaveChangesAsync()
        //{
        //    //return await DatabaseContext.SaveChangesAsync() >= 0;
        //    try
        //    {
        //        var saved = await DatabaseContext.SaveChangesAsync().ConfigureAwait(false) >= 0;
        //        return saved;
        //        // move on
        //    }
        //    catch (DbUpdateException e)
        //    {
        //        // get latest version of record for display
        //        return false;
        //    }
        //    catch (Exception ee)
        //    {
        //        return false;
        //    }
        //    //catch (DbUpdateConcurrencyException ex)
        //    //{
        //    //    //foreach (var entry in ex.Entries)
        //    //    //{
        //    //    //    if (entry.Entity is EntityBase)
        //    //    //    {
        //    //    //        var proposedValues = entry.CurrentValues;
        //    //    //        var databaseValues = entry.GetDatabaseValues();

        //    //    //        try
        //    //    //        {
        //    //    //            var rwNew = proposedValues.GetValue<byte[]>(nameof(EntityBase.RowVersion));
        //    //    //            var rwDb = databaseValues.GetValue<byte[]>(nameof(EntityBase.RowVersion));
        //    //    //            if (rwNew != null && rwDb != null)
        //    //    //            {
        //    //    //                rwNew = rwDb;
        //    //    //                var saved = await DatabaseContext.SaveChangesAsync().ConfigureAwait(false) >= 0;
        //    //    //                return saved;
        //    //    //            }
        //    //    //        }
        //    //    //        catch (Exception ex2)
        //    //    //        {

        //    //    //        }


        //    //    //        //foreach (IProperty property in proposedValues.Properties)
        //    //    //        //{
        //    //    //        //    var proposedValue = proposedValues[property];
        //    //    //        //    var databaseValue = databaseValues[property];

        //    //    //        //    // TODO: decide which value should be written to database
        //    //    //        //    // proposedValues[property] = <value to be saved>;
        //    //    //        //    if (property.Name == nameof(EntityBase.RowVersion))
        //    //    //        //    {
        //    //    //        //        proposedValue = databaseValue;
        //    //    //        //        var saved = await DatabaseContext.SaveChangesAsync().ConfigureAwait(false) >= 0;
        //    //    //        //        return saved;
        //    //    //        //    }
        //    //    //        //}

        //    //    //        //// Refresh original values to bypass next concurrency check
        //    //    //        //entry.OriginalValues.SetValues(databaseValues);
        //    //    //    }
        //    //    //    else
        //    //    //    {
        //    //    //        throw new NotSupportedException(
        //    //    //            "Don't know how to handle concurrency conflicts for "
        //    //    //            + entry.Metadata.Name);
        //    //    //    }
        //    //    //}
        //    //    //ex.Entries.Single().Reload();
        //    //    //var saved = await DatabaseContext.SaveChangesAsync().ConfigureAwait(false) >= 0;
        //    //    //return saved;
        //    //    return false;
        //    //}
        //    catch (Exception ee)
        //    {
        //        return false;
        //    }
        //}
        
        #endregion
    }
}
