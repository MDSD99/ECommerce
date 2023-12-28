using System;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Domain.AggregateModels.BuyerAggregate;
using OrderService.Domain.AggregateModels.OrderAggregate;
using OrderService.Domain.SeedWork;
using Polly;
using Polly.Retry;

#nullable disable

namespace OrderService.Infrastructure.Context
{
	public class OrderDbContextSeed
	{
        public async Task SeedAsync(OrderDbContext context, ILogger<OrderDbContext> logger)
        {
            var policy = CreatePolicy(logger, nameof(OrderDbContextSeed));

            await policy.ExecuteAsync(async () =>
            {
                var useCustomizationData = false;
                var contentRootPath = "Seeding/Setup";

                using (context)
                {
                    context.Database.Migrate();

                    if (!context.CardTypes.Any())
                    {
                        context.CardTypes.AddRange(useCustomizationData ? GetCardTypesFromFile(contentRootPath, logger) : GetPredefinedCardTypes());

                        await context.SaveChangeAsync();
                    }
                    if (!context.OrderStatus.Any())
                    {
                        context.OrderStatus.AddRange(useCustomizationData ? GetOrderStatusFromFile(contentRootPath, logger) : GetPredefinedOrderStatus());
                    }
                    await context.SaveChangeAsync();
                }
            });
        }
        private IEnumerable<CardType> GetCardTypesFromFile(string contentRootPath,ILogger<OrderDbContext> logger)
        {
            string FileName = "CardTypes.txt";
            if (!File.Exists(FileName))
            {
                return GetPredefinedCardTypes();
            }

            var fileContent = File.ReadAllLines(FileName);

            int id = 1;
            var list = fileContent.Select(i => new CardType(id++, i)).Where(i => i != null);
            return list;
        }

        private IEnumerable<CardType> GetPredefinedCardTypes()
        {
            return Enumeration.GetAll<CardType>();
        }
        private IEnumerable<OrderStatus> GetOrderStatusFromFile(string contentRootPath, ILogger<OrderDbContext> logger)
        {
            string FileName = "CardTypes.txt";
            if (!File.Exists(FileName))
            {
                return GetPredefinedOrderStatus();
            }

            var fileContent = File.ReadAllLines(FileName);

            int id = 1;
            var list = fileContent.Select(i => new OrderStatus(id++, i)).Where(i => i != null);
            return list;
        }

        private IEnumerable<OrderStatus> GetPredefinedOrderStatus()
        {
            return new List<OrderStatus>
            {
                OrderStatus.Submitted,
                OrderStatus.AwaitingValidation,
                OrderStatus.StockConfirmed,
                OrderStatus.Paid,
                OrderStatus.Shipped,
                OrderStatus.Cancelled
            };
        }

        private AsyncRetryPolicy CreatePolicy(ILogger<OrderDbContext> logger,string prefix,int retries = 3)
        {
            return Policy.Handle<SqlException>().WaitAndRetryAsync(retryCount: retries, sleepDurationProvider: retry => TimeSpan.FromSeconds(5), onRetry:
                (exception, timespan, retry, ctx) =>
                {
                    logger.LogWarning(exception,"[{prefix}] Exception {ExceptionType} with message {Message}",exception.Message);
                });
        }
    }
}

