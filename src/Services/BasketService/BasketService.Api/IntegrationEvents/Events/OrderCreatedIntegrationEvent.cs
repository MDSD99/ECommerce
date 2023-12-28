using System;
using BasketService.Api.Core.Domain.Models;
using EventBus.Base.Events;

#nullable disable

namespace BasketService.Api.IntegrationEvents.Events
{
	public class OrderCreatedIntegrationEvent:IntegrationEvent
	{
        public OrderCreatedIntegrationEvent(string userId, string userName,  string city, string street, string state, string country, string zipCode, string cardNumber, string cardHolderName, DateTime cardExpiration, string cardSecurityNumber, int cardTypeId, string buyer,CustomerBasket basket)
        {
           this.UserId = userId;
           this.UserName = userName;
           //this.OrderNumber = orderNumber;//int orderNumber,
           this.City = city;
           this.Street = street;
           this.State = state;
           this.Country = country;
           this.ZipCode = zipCode;
           this.CardNumber = cardNumber;
           this.CardHolderName = cardHolderName;
           this.CardExpiration = cardExpiration;
           this.CardSecurityNumber = cardSecurityNumber;
           this.CardTypeId = cardTypeId;
           this.Buyer = buyer;
           this.Basket = basket;
        }

        public string UserId { get; set; }
		public string UserName { get; set; }
		//public int OrderNumber { get; set; }
		public string City { get; set; }
		public string Street { get; set; }
		public string State { get; set; }
		public string Country { get; set; }
		public string ZipCode { get; set; }
		public string CardNumber { get; set; }
		public string CardHolderName { get; set; }
		public DateTime CardExpiration { get; set; }
		public string CardSecurityNumber { get; set; }
		public int CardTypeId { get; set; }
		public string Buyer { get; set; }
		public CustomerBasket Basket { get; }
	}
}

