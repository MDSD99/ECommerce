using MediatR;
using OrderService.Application.Dtos;
using OrderService.Domain.Models;

#nullable disable

namespace OrderService.Application.Features.Commands.CreateOrder
{
	public class CreateOrderCommand:IRequest<bool>
	{
		private readonly List<OrderItemDto> orderItems;

		public string UserName { get; private set; }
		public string City { get; private set; }
		public string Street { get; private set; }
		public string State { get; private set; }
		public string Country { get; private set; }
		public string Zipcode { get; private set; }
		public string Cardnumber { get; private set; }
		public string CardHolderName { get; private set; }
		public DateTime CardExpiration { get; private set; }
		public string CardSecurityNumber { get; private set; }
		public int CardTypeId { get; set; }
		public IEnumerable<OrderItemDto> OrderItems => orderItems;

		public CreateOrderCommand()
		{
			orderItems = new List<OrderItemDto>();
		}
		public CreateOrderCommand(List<BasketItem> basketItems,string userId,string userName,string city,string street,string state,string country,string zipcode,string cardNumber,string cardHolderName,DateTime cardExpiration,string cardSecurityNumber,int cardTypeId) : this()
		{
			var dtoList = basketItems.Select(item => new OrderItemDto()
			{
				ProductId=item.ProductId,
				ProductName=item.ProductName,
				PictureUrl=item.PictureUrl,
				UnitPrice=item.UnitPrice,
				Units=item.Quantity
			});

			orderItems = dtoList.ToList();

			this.UserName = userId;
			this.City = city;
			this.Street = street;
			this.State=state;
			this.Country=country;
			this.Zipcode = zipcode;
			this.Cardnumber = cardNumber;
			this.CardHolderName = cardHolderName;
			this.CardExpiration = cardExpiration;
			this.CardSecurityNumber = cardSecurityNumber;
			this.CardTypeId = cardTypeId;
		}
	}
}

