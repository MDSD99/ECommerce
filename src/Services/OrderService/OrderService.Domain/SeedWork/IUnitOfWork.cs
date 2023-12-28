namespace OrderService.Domain.SeedWork
{
	public interface IUnitOfWork
	{
		Task<int> SaveChangeAsync(CancellationToken cancellationToken = default(CancellationToken));

		Task<bool> SaveEntitiesAsync(CancellationToken cancellationToken = default(CancellationToken));
	}
}