using System;
using MediatR;
using OrderService.Domain.AggregateModels.BuyerAggregate;

namespace OrderService.Domain.Events
{
	public class BuyerAndPaymentMethodVerifiedDomainEvent:INotification
	{
        public BuyerAndPaymentMethodVerifiedDomainEvent(Buyer buyer, PaymentMethod payment, Guid orderId)
        {
            Buyer = buyer;
            Payment = payment;
            OrderId = orderId;
        }

        public Buyer Buyer { get; private set; }
		public PaymentMethod Payment { get; private set; }
		public Guid OrderId { get; private set; }

	}
}

