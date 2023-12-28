using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Polly;

#nullable disable

namespace OrderService.Api.Extensions
{
	public static class HostExtension
	{
		public static IServiceCollection MigrateDbContext<TContext>(this IServiceCollection host,Action<TContext,IServiceProvider> seeder) where TContext:DbContext
		{
			using(var scope=host.BuildServiceProvider().CreateScope())
			{
				var service = scope.ServiceProvider;

				var logger = service.GetRequiredService<ILogger<TContext>>();

				var context = service.GetService<TContext>();

				try
				{
					logger.LogInformation("Migration database associated with context {DbContextName}",typeof(TContext).Name);

					var policy = Policy.Handle<SqlException>().WaitAndRetry(new TimeSpan[]
					{
						TimeSpan.FromSeconds(3),
                        TimeSpan.FromSeconds(5),
                        TimeSpan.FromSeconds(8)
                    });

					policy.Execute(() =>
					{
						InvokeSeeker(seeder, context, service);
					});

					logger.LogInformation("Migrate database ");
				}
				catch //(Exception ex)
				{
					logger.LogError("An error occured while migration the database used on context {DbContextName}",typeof(TContext).Name);
				}

				return host;
			}
		}

		public static void InvokeSeeker<TContext>(Action<TContext,IServiceProvider> seeder,TContext context,IServiceProvider services) where TContext:DbContext
		{
			context.Database.EnsureCreated();
			context.Database.Migrate();
			seeder(context, services);
		}
	}
}

