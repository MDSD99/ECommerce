using System;
using EventBus.AzureServiceBus;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.RabbitMQ;

namespace EventBus.Factory
{
	public static class EventBusFactory
	{
		public static IEventBus Create(EventBusConfig eventBusConfig,IServiceProvider serviceProvider)
		=> eventBusConfig.EventBusType switch
			{
				EventBusType.AzureServiceBus => new EventBusServiceBus(serviceProvider, eventBusConfig),
				EventBusType.RabbitMQ=> new EventBusRabbitMQ(serviceProvider, eventBusConfig),
                _ => new EventBusRabbitMQ(serviceProvider, eventBusConfig)
			} ;
	}
}

