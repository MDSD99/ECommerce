using CatalogService.Api.Infrastructure.Context;
using Microsoft.EntityFrameworkCore;

namespace CatalogService.Api.Infrastructure.Extensions
{
    public static class DbContextRegistration
	{
		public static IServiceCollection ConfigureDbContext(this IServiceCollection services,IConfiguration configuration)
        {
            var connStr = configuration["ConnectionStrings:CatalogDb"];
            services.AddDbContext<CatalogContext>(options =>
				{
                    options.UseSqlServer(connStr, sqlServerOptionsAction: sqlOptions =>
					{
						//sqlOptions.MigrationsAssembly(typeof(Program).GetTypeInfo().Assembly.GetName().Name);

						sqlOptions.EnableRetryOnFailure(maxRetryCount:15,maxRetryDelay:TimeSpan.FromSeconds(30),errorNumbersToAdd:null);
					});
				});
			return services;
		}
	}
}

