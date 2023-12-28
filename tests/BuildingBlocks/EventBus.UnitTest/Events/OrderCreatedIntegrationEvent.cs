using System;
using EventBus.Base.Events;

#nullable disable
namespace EventBus.UnitTest.Events
{
	public class OrderCreatedIntegrationEvent:IntegrationEvent
	{
		public OrderCreatedIntegrationEvent(int id)
		{
			this.id = id;
		}
		public int id { get; set; }
	}
}
