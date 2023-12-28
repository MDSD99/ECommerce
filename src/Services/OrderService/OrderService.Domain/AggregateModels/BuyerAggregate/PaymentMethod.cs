using System;
using OrderService.Domain.Exceptions;
using OrderService.Domain.SeedWork;

#nullable disable

namespace OrderService.Domain.AggregateModels.BuyerAggregate
{
	public class PaymentMethod : BaseEntity
	{
		public string Alias { get; set; }

		public string CardNumber { get; set; }

		public string SecurityNumber { get; set; }

		public string CardHolderName { get; set; }

		public DateTime Expiration { get; set; }

		public int CardTypeId { get; set; }

		public CardType CardType { get; private set; }

		public PaymentMethod() { }

        public PaymentMethod(int cardTypeId,string alias, string cardNumber, string securityNumber, string cardHolderName, DateTime expiration)
        {
            this.CardNumber = !String.IsNullOrWhiteSpace(CardNumber) ? cardNumber : throw new OrderingDomainException(nameof(cardNumber));
            this.SecurityNumber = !String.IsNullOrWhiteSpace(securityNumber) ? securityNumber : throw new OrderingDomainException(nameof(securityNumber));
            this.CardHolderName = !String.IsNullOrWhiteSpace(cardHolderName) ? cardHolderName : throw new OrderingDomainException(nameof(cardHolderName));

            if(expiration < DateTime.UtcNow)
            {
                throw new OrderingDomainException(nameof(expiration));
            }

            this.Alias = alias;
            this.Expiration = expiration;
            this.CardTypeId = cardTypeId;
        }

        public bool IsEqualTo(int cardTypeId, string cardNumber, DateTime expiration)
        => CardTypeId == cardTypeId && CardNumber == cardNumber && Expiration == expiration;
    }
}

