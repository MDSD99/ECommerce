using System;
using OrderService.Api.IntegrationEvents.EventHandlers;

namespace OrderService.Api.Extensions.EventHandlerRegistration
{
	public static class EventHandlerRegistration
	{
		public static IServiceCollection ConfigureEventHandlers(this IServiceCollection services)
		{
			services.AddTransient<OrderCreatedIntegrationEventHandler>();

			return services;
		}
	}
}

