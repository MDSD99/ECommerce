using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.EntitiyConfigurations
{
	public class OrderEntityConfiguration:IEntityTypeConfiguration<Order>
	{
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("orders", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(s => s.Id);
            builder.Property(s => s.Id).ValueGeneratedOnAdd();
            builder.Ignore(i => i.DomainEvents);
            builder.OwnsOne(s => s.Address, a => { a.WithOwner(); });
            builder.Property<int>("orderStatusId")
                .UsePropertyAccessMode(PropertyAccessMode.Field)
                .HasColumnName("OrderStatusId")
                .IsRequired();

            var navigation = builder.Metadata.FindNavigation(nameof(Order.OrderItems));
            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);

            builder.HasOne(o => o.Buyer).WithMany().HasForeignKey(o=>o.BuyerId);

            builder.HasOne(o => o.OrderStatus).WithMany().HasForeignKey("orderStatusId");
        }
    }
}

