using System;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.Context;

#nullable disable

namespace OrderService.Infrastructure.Repositories
{
	public class GenericRepository<T>:IGenericRepository<T> where T:BaseEntity
	{
        private readonly OrderDbContext orderDbContext;
        public GenericRepository(OrderDbContext orderDbContext)
        {
            this.orderDbContext = orderDbContext;
        }

        public IUnitOfWork UnitOfWork { get; }

        public virtual async Task<T> AddAsync(T entity)
        {
            await orderDbContext.Set<T>().AddAsync(entity);
            return entity;
        }

        public virtual async Task<List<T>> Get(Expression<Func<T, bool>> filter, Func<IQueryable<T>, IOrderedQueryable<T>> orderby, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = orderDbContext.Set<T>();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }
            if (filter != null)
            {
                query.Where(filter);
            }
            if (orderby != null)
            {
                query = orderby(query);
            }

            return await query.ToListAsync();
        }

        public virtual Task<List<T>> Get(Expression<Func<T, bool>> filter, params Expression<Func<T, object>>[] includes)
        {
            return Get(filter, null, includes);
        }

        public virtual async Task<List<T>> GetAll()
        {
            return await orderDbContext.Set<T>().ToListAsync();
        }

        public virtual async Task<T> GetById(Guid id)
        {
            return await orderDbContext.Set<T>().FindAsync(id);
        }

        public virtual async Task<T> GetByIdAsync(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = orderDbContext.Set<T>();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.FirstOrDefaultAsync(s => s.Id == id);
        }

        public virtual async Task<T> GetSingleAsync(Expression<Func<T, bool>> expression, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = orderDbContext.Set<T>();

            foreach (var item in includes)
            {
                query = query.Include(item);
            }

            return await query.SingleOrDefaultAsync(expression);
        }

        public virtual T Update(T entity)
        {
            orderDbContext.Set<T>().Update(entity);
            return entity; 
        }
    }
}

