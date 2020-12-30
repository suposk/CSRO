﻿using Microsoft.EntityFrameworkCore;
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

        Task<TModel> GetAsync(int id);        

        Task<List<TModel>> GetAllAsync();
        void Add(TModel entity);
        void Remove(TModel entity);
        void Update(TModel entity);

        Task<bool> SaveChangesAsync();
        Task<TModel> GetByFilter(Expression<Func<TModel, bool>> expression, params Expression<Func<TModel, object>>[] includes);
        Task<TModel> GetByFilter(Expression<Func<TModel, bool>> expression);
    }
}
