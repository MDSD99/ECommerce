using System;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.Events;
using OrderService.Domain.SeedWork;

#nullable disable

namespace OrderService.Domain.AggregateModels.OrderAggregate
{
	public class Order:BaseEntity,IAggregateRoot
	{
		public DateTime OrderDate { get; private set; }

		public int Quantity { get; private set; }

		public string Description { get; private set; }

		public Guid? BuyerId { get; private set; }

		public Buyer Buyer { get; private set; }

		public Address Address { get; private set; }

		private int orderStatusId;

		public OrderStatus OrderStatus { get; private set; }

		private readonly List<OrderItem> orderItems;

		public IReadOnlyCollection<OrderItem> OrderItems => orderItems;

		public Guid? PaymentMethodId { get; set; }

		protected Order()
		{
			Id = Guid.NewGuid();

			orderItems = new List<OrderItem>();
		}

		public Order(string userName,Address address,int cardTypeId,string cardNumber,string cardSecurityNumber,string cardHolderName,DateTime cardExpiration,Guid? paymentMethodId,Guid? buyerId = null) : this()
		{
			this.BuyerId = buyerId;
			this.orderStatusId = OrderStatus.Submitted.Id;
			this.OrderDate = DateTime.UtcNow;
			this.Address = address;
			this.PaymentMethodId = paymentMethodId;

			AddOrderStartedDomainEvent(userName,cardTypeId,cardNumber,cardSecurityNumber,cardHolderName,cardExpiration);
        }

		private void AddOrderStartedDomainEvent(string userName,int cardTypeId,string cardNumber,string cardSecurityNumber,string cardHolderName,DateTime cardExpiration)
		{
			var orderstartedDomainEvent = new OrderStartedDomainEvent(userName, cardTypeId, cardNumber, cardSecurityNumber, cardHolderName, cardExpiration,this);

			this.AddDomainEvent(orderstartedDomainEvent);
		}

		public void AddOrderItem(int productId,string productName,decimal unitPrice,string pictureUrl,int units = 1)
		{
			var orderItem = new OrderItem(productId, productName, pictureUrl, unitPrice,  units);

			orderItems.Add(orderItem);
		}

		public void SetBuyerId(Guid buyerId)
		{
			this.BuyerId = buyerId;
		}

		public void SetPaymentMethodId(Guid paymentMethodId)
		{
			this.PaymentMethodId = paymentMethodId;
		}
    }
}

