using System;
using Newtonsoft.Json;

namespace EventBus.Base.Events
{
	public class IntegrationEvent
	{
		//Bundan sonra RabbitMQ ile veya AzureServiceBus arasında kullanılan objelerimiz diyebiliriz.
		//Base Class
		[JsonProperty]
		public Guid Id { get; private set; }

		[JsonProperty]
		public DateTime CreatedDate { get; private set; }

		public IntegrationEvent()
		{
			Id = Guid.NewGuid();
			CreatedDate = DateTime.Now;
		}

        [JsonConstructor]
        public IntegrationEvent(Guid id,DateTime dateTime)
		{
			this.Id = id;
			this.CreatedDate = dateTime;
		}
	}
}

