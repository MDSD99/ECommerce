using System;
using EventBus.Base.Abstraction;

namespace EventBus.UnitTest.Events.EventHandler
{
    public class OrderCratedIntegrationEventHandler : IIntegrationEventHandler<OrderCreatedIntegrationEvent>
	{
		public OrderCratedIntegrationEventHandler()
		{
		}

        public Task Handle(OrderCreatedIntegrationEvent @event)
        {
            Console.WriteLine("Handle methot worked. With Event id : ",@event.Id);
            return Task.CompletedTask;
        }
    }
}

