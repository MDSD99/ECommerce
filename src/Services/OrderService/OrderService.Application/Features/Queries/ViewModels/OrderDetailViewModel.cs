#nullable disable

namespace OrderService.Application.Features.Queries.ViewModels
{
	public class OrderDetailViewModel
	{
		public string OrderNumber { get; init; }
		public DateTime Date { get; init; }
		public string Status { get; init; }
		public string Description { get; init; }
		public string Street { get; init; }
		public string City { get; init; }
		public string Zipcode { get; init; }
		public string Country { get; init; }
		public List<OrderDetailItemViewModel> OrderItems { get; set; }
		public decimal Total { get; set; }
	}
}

