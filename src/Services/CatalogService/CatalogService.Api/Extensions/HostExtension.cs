
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

#nullable disable
namespace CatalogService.Api.Infrastructure.Extensions
{
	public static class HostExtension
	{
		public static IServiceCollection MigrateDbContext<TContext>(this IServiceCollection webHost,Action<TContext,IServiceProvider> seeder) where TContext:DbContext
		{
			using(var scope=webHost.BuildServiceProvider().CreateAsyncScope())
			{
				var services = scope.ServiceProvider;

				var logger = services.GetRequiredService<ILogger<TContext>>();

				var context = services.GetService<TContext>();

				try
				{
					logger.LogInformation("Migrating database associated with context {DbContextName}",typeof(TContext).Name);

					var retry = Policy.Handle<SqlException>()
						.WaitAndRetry(new TimeSpan[]
						{
							TimeSpan.FromSeconds(3),
							TimeSpan.FromSeconds(5),
							TimeSpan.FromSeconds(8)
						});

					retry.Execute(() => InvokeSeeder(seeder,context,services));

					logger.LogInformation("Migrating database associated with context {DbContextName}",typeof(TContext).Name);
					
				}
				catch (Exception ex)
				{
					logger.LogError(ex,"An error occured while migrating the database used on context {DbContextName}",typeof(TContext).Name);
				}
				return webHost;
			}
		}

		private static void InvokeSeeder<TContext>(Action<TContext,IServiceProvider> seeder,TContext context,IServiceProvider serviceProvider) where TContext:DbContext
		{
			context.Database.EnsureCreated();
			context.Database.Migrate();
			seeder(context, serviceProvider);
		}
	}
}

