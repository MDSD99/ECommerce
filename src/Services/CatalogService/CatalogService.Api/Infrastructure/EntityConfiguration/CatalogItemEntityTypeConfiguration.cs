using System;
using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfiguration
{
	public class CatalogItemEntityTypeConfiguration: IEntityTypeConfiguration<CatalogItem>
    {
        public void Configure(EntityTypeBuilder<CatalogItem> builder)
        {
            builder.ToTable("CatalogItem", CatalogContext.DEFAULT_SCHEMA);

            builder.HasKey(s => s.Id);

            //builder.Property(s => s.Id).UseHiLo("catalog_hilo").IsRequired();

            builder.Property(s => s.Name).IsRequired(true).HasMaxLength(50);

            builder.Property(s => s.AvaibleStock).IsRequired();

            builder.Property(s => s.OnReorder).IsRequired();

            builder.Property(s => s.Price).IsRequired(true);

            builder.Property(s => s.PictureFileName).IsRequired(false);

            builder.Ignore(s => s.PictureUri);

            builder.HasOne(s => s.CatalogBrand).WithMany().HasForeignKey(s=>s.CatalogBrandId);

            builder.HasOne(s => s.CatalogType).WithMany().HasForeignKey(s => s.CatalogTypeId);
        }
    }
}

