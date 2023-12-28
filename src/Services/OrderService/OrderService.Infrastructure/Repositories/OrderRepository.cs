using System;
using System.Linq.Expressions;
using OrderService.Application.Interfaces.Repositories;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Context;

#nullable disable

namespace OrderService.Infrastructure.Repositories
{
    public class OrderRepository : GenericRepository<Order>, IOrderRepository
    {
        private readonly OrderDbContext orderDbContext;
        public OrderRepository(OrderDbContext orderDbContext) : base(orderDbContext)
        {
            this.orderDbContext = orderDbContext;
        }
        public override async Task<Order> GetByIdAsync(Guid id, params Expression<Func<Order, object>>[] includes)
        {
            var entity=await base.GetByIdAsync(id, includes);

            if (entity == null)
            {
                entity = orderDbContext.Orders.Local.FirstOrDefault(s => s.Id == id);
            }

            return entity;
        }
    }
}

