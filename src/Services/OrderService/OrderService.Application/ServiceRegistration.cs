using System.Reflection;
using MediatR;
using Microsoft.Extensions.DependencyInjection; 

namespace OrderService.Application
{
	public static class ServiceRegistration
	{
		public static IServiceCollection AddApplicationRegistration(this IServiceCollection services)//,Type startup
        {
			//var assm = Assembly.GetExecutingAssembly();
            //var assm2 = startup.GetTypeInfo().Assembly;

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            services.AddMediatR(Assembly.GetExecutingAssembly());
            //services.AddMediatR(Assembly.GetExecutingAssembly(), typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly);
            //services.AddMediatR(assm, typeof(CreateOrderCommandHandler).GetTypeInfo().Assembly);//,assm2, 
            return services;
		}
	}
}

