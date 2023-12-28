using System;
using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Domain.Models;
using Newtonsoft.Json;
using StackExchange.Redis;

#nullable disable

namespace BasketService.Api.Infrastructure.Repository
{
	public class RedisBasketRepository:IBasketRepository
	{

        private readonly ILogger<RedisBasketRepository> logger;
        private readonly ConnectionMultiplexer redis;
        private readonly IDatabase database;

        public RedisBasketRepository(ILoggerFactory logger, ConnectionMultiplexer redis)
        {
            this.logger = logger.CreateLogger<RedisBasketRepository>();
            this.redis = redis;
            this.database = redis.GetDatabase();
        }

        public async Task<bool> DeleteBasket(string id)
        {
            return await database.KeyDeleteAsync(id);
        }

        public async Task<CustomerBasket> GetBasket(string customerId)
        {
            var data = await database.StringGetAsync(customerId);

            if(data.IsNullOrEmpty)
            {
                return null;
            }

            return JsonConvert.DeserializeObject<CustomerBasket>(data);
        }

        public IEnumerable<string> GetUsers()
        {
            var server = GetServer();
            var data = server.Keys();
            return data.Select(s => s.ToString());
        }

        public async Task<CustomerBasket> UpdateBasket(CustomerBasket basket)
        {
            var created = await database.StringSetAsync(basket.BuyerId, JsonConvert.SerializeObject(basket));

            if (!created)
            {
                logger.LogInformation("Problem occur persisting the item.");
                return null;
            }
            logger.LogInformation("Basket item persisted succesfully");

            return await GetBasket(basket.BuyerId);
        }

        private IServer GetServer()
        {
            var endpoint = redis.GetEndPoints();
            return redis.GetServer(endpoint.First());
        }
    }
}

