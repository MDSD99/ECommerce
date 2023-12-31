﻿#nullable disable

namespace BasketService.Api.Core.Domain.Models
{
	public class CustomerBasket
	{
		public string BuyerId { get; set; }

		public List<BasketItem> Items { get; set; } = new List<BasketItem>();

		public CustomerBasket()
		{

		}
		public CustomerBasket(string customerId)
		{
			this.BuyerId = customerId;
		}
	}
}

