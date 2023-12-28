using System;
using EventBus.Base.Events;

namespace NotificationService.IntegrationEvents.Events
{
	public class OrderPaymentFailedIntegrationEvent:IntegrationEvent
	{
        public int OrderId { get; set; }

        public string ErrorMessage { get; set; }

        public OrderPaymentFailedIntegrationEvent(int id,string errormessage)
        {
            this.OrderId = id;
            this.ErrorMessage = errormessage;
        }
    }
}

