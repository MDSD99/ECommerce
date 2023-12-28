using System;
using MediatR;
using Microsoft.EntityFrameworkCore;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.EntitiyConfigurations;
using OrderService.Infrastructure.Extensions;

#nullable disable

namespace OrderService.Infrastructure.Context
{
	public class OrderDbContext:DbContext,IUnitOfWork
	{
		public const string DEFAULT_SCHEMA = "ORDER";
        private readonly IMediator mediator;

        public virtual DbSet<Order> Orders { get; set; }
		public virtual DbSet<OrderItem> OrderItems { get; set; }
		public virtual DbSet<PaymentMethod> PaymentMethods { get; set; }
		public virtual DbSet<Buyer> Buyers { get; set; }
		public virtual DbSet<CardType> CardTypes { get; set; }
		public virtual DbSet<OrderStatus> OrderStatus { get; set; }

        public OrderDbContext() : base()
        {

        }
        public OrderDbContext(DbContextOptions<OrderDbContext> options,IMediator mediator) : base(options)
        {
            this.mediator = mediator;
        }

        public async Task<int> SaveChangeAsync(CancellationToken cancellationToken = default)
        {
            await mediator.DispatchDomainEventAsync(this);

            int id=await base.SaveChangesAsync(cancellationToken);

            return id;
        }

        public async Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default)
        {
            await mediator.DispatchDomainEventAsync(this);

            await base.SaveChangesAsync(cancellationToken);

            return true;
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            //base.OnModelCreating(modelBuilder);
			modelBuilder.ApplyConfiguration(new OrderEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderItemEntityConfiguration());
            modelBuilder.ApplyConfiguration(new BuyerEntityConfiguration());
            modelBuilder.ApplyConfiguration(new OrderStatusEntityConfiguration());
            modelBuilder.ApplyConfiguration(new PaymentMethodEntityConfiguration());
            modelBuilder.ApplyConfiguration(new CardTypeEntityConfiguration());
        }
    }
}

