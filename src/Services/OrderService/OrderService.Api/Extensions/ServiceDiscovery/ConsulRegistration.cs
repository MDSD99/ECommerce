using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

#nullable disable

namespace OrderService.Api.Extensions.ServiceDiscovery
{
	public static class ConsulRegistration
	{
		public static IServiceCollection AddServiceDiscoveryRegistration(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddSingleton<IConsulClient, ConsulClient>
				(p=>new ConsulClient(consulConfig =>
                {
                    var address = configuration["ConsulConfig:Address"];
                    consulConfig.Address = new Uri(address);
                }));
			return services;
		}
        public static IApplicationBuilder RegisterBuilder(this IApplicationBuilder app,IHostApplicationLifetime lifetime)
		{
			var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

			var logginFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

			var logger = logginFactory.CreateLogger<IApplicationBuilder>();

			var features = app.Properties["server.Features"] as FeatureCollection;

			var addresses = features.Get<IServerAddressesFeature>();

			var address = addresses.Addresses.First();

			var uri = new Uri(address);

			var registration = new AgentServiceRegistration()
			{
				ID = $"OrderService",
				Name = $"OrderService",
				Address = $"{uri.Host}",
				Port=uri.Port,
				Tags = new[] {"Ordering Service","Order"} 
			};

			logger.LogInformation("Regitering with Consul");

			consulClient.Agent.ServiceDeregister(registration.ID).Wait();

			consulClient.Agent.ServiceRegister(registration).Wait();

			lifetime.ApplicationStopping.Register(() =>
			{
				logger.LogInformation("Deregistering from Consul");
				consulClient.Agent.ServiceDeregister(registration.ID).Wait();
			});

			return app;
		}
	}
}

