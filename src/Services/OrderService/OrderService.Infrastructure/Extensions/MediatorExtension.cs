using System;
using MediatR;
using OrderService.Domain.SeedWork;
using OrderService.Infrastructure.Context;

namespace OrderService.Infrastructure.Extensions
{
	public static class MediatorExtension
	{
		public static async Task DispatchDomainEventAsync(this IMediator mediator,OrderDbContext orderDbContext)
		{
			var domainEntities = orderDbContext.ChangeTracker.Entries<BaseEntity>().Where(s => s.Entity.DomainEvents != null && s.Entity.DomainEvents.Any());

			var domainEvents = domainEntities.SelectMany(s => s.Entity.DomainEvents).ToList();

			domainEntities.ToList().ForEach(entity => entity.Entity.ClearDomainEvents());

			foreach (var item in domainEvents)
			{
				await mediator.Publish(item);
			}
		}
	}
}

