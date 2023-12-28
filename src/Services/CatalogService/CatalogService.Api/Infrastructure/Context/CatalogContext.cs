using CatalogService.Api.Core.Domain;
using CatalogService.Api.Infrastructure.EntityConfiguration;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Infrastructure.Context
{
	public class CatalogContext:DbContext
	{
		public const string DEFAULT_SCHEMA = "CATALOG";

		public CatalogContext(DbContextOptions<CatalogContext> options):base(options)
		{

		}

		public virtual DbSet<CatalogType> CatalogTypes { get; set; }
		public virtual DbSet<CatalogBrand> CatalogBrands { get; set; }
		public virtual DbSet<CatalogItem> CatalogItems { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
			base.OnModelCreating(modelBuilder);

			modelBuilder.ApplyConfiguration(new CatalogBrandEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new CatalogItemEntityTypeConfiguration());

            modelBuilder.ApplyConfiguration(new CatalogTypeEntityTypeConfiguration());

			//base.OnModelCreating(modelBuilder);
        }
    }
}

