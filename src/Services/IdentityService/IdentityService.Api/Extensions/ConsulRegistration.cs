using System;
using Consul;
using Microsoft.AspNetCore.Hosting.Server.Features;
using Microsoft.AspNetCore.Http.Features;

#nullable disable
namespace IdentityService.Api.Extensions
{
	public  static class ConsulRegistration
	{
		public static IServiceCollection ConfigureConsul(this IServiceCollection services,IConfiguration configuration)
		{
			services.AddSingleton<IConsulClient>(p => new ConsulClient(ConsulConfig =>
			{
				var address = configuration["ConsulConfig:Address"];
				ConsulConfig.Address = new Uri(address);
			}));
			return services;
		}

		public static IApplicationBuilder RegisterWithConsul(this IApplicationBuilder app,IHostApplicationLifetime lifeTime)
		{
			var consulClient = app.ApplicationServices.GetRequiredService<IConsulClient>();

			var logginFactory = app.ApplicationServices.GetRequiredService<ILoggerFactory>();

			var logger = logginFactory.CreateLogger<IApplicationBuilder>();

			var features = app.Properties["server.Features"] as FeatureCollection;
			var addresses = features.Get<IServerAddressesFeature>();
			var adress = addresses.Addresses.First();

			var uri = new Uri(adress);

			var registration = new AgentServiceRegistration()
			{
				ID=$"IdentityService",
				Name=$"IdentityService",
				Address=$"{uri.Host}",
				Port=uri.Port,
				Tags = new[] {"Identity Service","Identity","Token","JWT"} 
			};

			logger.LogInformation("Registering with Consul");
			consulClient.Agent.ServiceDeregister(registration.ID).Wait();
			consulClient.Agent.ServiceRegister(registration).Wait();

			lifeTime.ApplicationStopping.Register(() =>
			{
				logger.LogInformation("Deregistering from Consul");
				consulClient.Agent.ServiceDeregister(registration.ID).Wait();
			});

			return app;
		}
	}
}

