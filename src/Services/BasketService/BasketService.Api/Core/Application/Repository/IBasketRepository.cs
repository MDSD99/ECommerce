using BasketService.Api.Core.Domain.Models;

namespace BasketService.Api.Core.Application.Repository
{
	public interface IBasketRepository
	{
		Task<CustomerBasket> GetBasket(string customerId);
		IEnumerable<string> GetUsers();
		Task<CustomerBasket> UpdateBasket(CustomerBasket basket);
		Task<bool> DeleteBasket(string id);
	}
}

