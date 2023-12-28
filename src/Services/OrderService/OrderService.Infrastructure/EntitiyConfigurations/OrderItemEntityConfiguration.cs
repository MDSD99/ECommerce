using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntitiyConfigurations
{
	public class OrderItemEntityConfiguration:IEntityTypeConfiguration<OrderItem>
	{
        public void Configure(EntityTypeBuilder<OrderItem> builder)
        {
            builder.ToTable("orderItems", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(s => s.Id);

            builder.Ignore(s => s.DomainEvents);

            builder.Property(p => p.Id).ValueGeneratedOnAdd();

            builder.Property<int>("OrderId").IsRequired();
        }
    }
}

