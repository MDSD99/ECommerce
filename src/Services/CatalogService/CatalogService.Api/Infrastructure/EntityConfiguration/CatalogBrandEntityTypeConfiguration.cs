using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace CatalogService.Api.Infrastructure.EntityConfiguration
{
	public class CatalogBrandEntityTypeConfiguration:IEntityTypeConfiguration<CatalogBrand>
	{
        public void Configure(EntityTypeBuilder<CatalogBrand> builder)
        {
            builder.ToTable("CatalogBrand",CatalogContext.DEFAULT_SCHEMA);

            builder.HasKey(s => s.Id);

           // builder.Property(s => s.Id).UseHiLo("brand_hilo").IsRequired();

            builder.Property(s => s.Brand).IsRequired().HasMaxLength(100);
        }
    }
}

