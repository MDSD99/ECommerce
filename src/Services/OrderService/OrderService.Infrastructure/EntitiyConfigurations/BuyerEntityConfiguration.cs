﻿using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Infrastructure.Context;
using Microsoft.EntityFrameworkCore.SqlServer;
using System.Data;

namespace OrderService.Infrastructure.EntitiyConfigurations
{
	public class BuyerEntityConfiguration:IEntityTypeConfiguration<Buyer>
	{
        public void Configure(EntityTypeBuilder<Buyer> builder)
        {
            builder.ToTable("buyers", OrderDbContext.DEFAULT_SCHEMA);

            builder.HasKey(b => b.Id);

            builder.Ignore(b => b.DomainEvents);

            builder.Property(b => b.Id).ValueGeneratedOnAdd();

            builder.Property(b => b.Name).HasColumnType("name").HasColumnType(SqlDbType.NVarChar.ToString()).HasMaxLength(100);

            builder.HasMany(s => s.PaymentMethods).WithOne().HasForeignKey(i => i.Id).OnDelete(DeleteBehavior.Cascade);

            var navigation = builder.Metadata.FindNavigation(nameof(Buyer.PaymentMethods));

            navigation?.SetPropertyAccessMode(PropertyAccessMode.Field);
        }
    }
}

