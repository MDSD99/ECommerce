using System;
using OrderService.Domain.SeedWork;
using OrderService.Domain.Events;

#nullable disable

namespace OrderService.Domain.AggregateModels.BuyerAggregate
{
	public class Buyer : BaseEntity,IAggregateRoot
	{
		public string Name { get; set; }

		private List<PaymentMethod> paymentMethods;

		public IEnumerable<PaymentMethod> PaymentMethods => paymentMethods.AsReadOnly();

		protected Buyer()
		{
			paymentMethods = new List<PaymentMethod>();
		}

		public Buyer(string userName)
		{
			this.Name = userName;
		}

        public PaymentMethod VerifyOrAddPaymentMethod(int cardTypeId,string alias,string cardNumber,string securityNumber,string cardHolderName,DateTime expiration,Guid orderId)
		{
			var existingPayment = paymentMethods.SingleOrDefault(s => s.IsEqualTo(cardTypeId, cardNumber, expiration));

			if (existingPayment != null)
			{
				AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, existingPayment, orderId));

				return existingPayment;
			}
			var payment = new PaymentMethod(cardTypeId, alias, cardNumber, securityNumber, cardHolderName, expiration);

			paymentMethods.Add(payment);

			AddDomainEvent(new BuyerAndPaymentMethodVerifiedDomainEvent(this, payment, orderId));

			return payment;
        }

		public override bool Equals(object obj) => base.Equals(obj) || (obj is Buyer buyer && Id.Equals(buyer.Id) && Name == buyer.Name);

        public override int GetHashCode() => base.GetHashCode();
	}
}

