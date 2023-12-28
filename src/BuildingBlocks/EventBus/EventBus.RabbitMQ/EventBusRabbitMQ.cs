using System.Net.Sockets;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Newtonsoft.Json;
using Polly;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using RabbitMQ.Client.Exceptions;

#nullable disable
namespace EventBus.RabbitMQ
{
    public class EventBusRabbitMQ : BaseEventBus
    {
        private readonly RabbitMQPersistenceConnection rabbitMQPersistenceConnection;

        private readonly IConnectionFactory connectionFactory;

        private readonly IModel consumerChannel;

        public EventBusRabbitMQ(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            if (config.Connection != null)
            {
                var connJson = JsonConvert.SerializeObject(config.Connection, new JsonSerializerSettings
                {
                    //Aynı isimde iç içe birden fazla obje olduğunda hata alınır.
                    ReferenceLoopHandling = ReferenceLoopHandling.Ignore
                });

                connectionFactory = JsonConvert.DeserializeObject<ConnectionFactory>(connJson);
            }
            else
                connectionFactory = new ConnectionFactory();

            rabbitMQPersistenceConnection = new RabbitMQPersistenceConnection(connectionFactory,config.ConnectionRetryCount);

            consumerChannel = CreateConsumerChannel();

            SubsManager.OnEventRemoved += SubsManager_OnEventRemoved;
        }

        private void SubsManager_OnEventRemoved(object sender, string eventName)
        {
            eventName = ProcessEventName(eventName);

            if (!rabbitMQPersistenceConnection.IsConnected)
            {
                rabbitMQPersistenceConnection.TryConnect();
            }

            consumerChannel.QueueUnbind(queue: eventName, exchange: "direct", routingKey: eventName,arguments: null);

            if (SubsManager.IsEmpty)
            {
                consumerChannel.Close();
            }
        }

        public override void Publish(IntegrationEvent @event)
        {
            if (!rabbitMQPersistenceConnection.IsConnected)
            {
                rabbitMQPersistenceConnection.TryConnect();
            }

            var policy = Policy.Handle<BrokerUnreachableException>().Or<SocketException>().
                WaitAndRetry(
                eventBusConfig.ConnectionRetryCount,
                retryAttempt=>TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                (ex, time) =>
                {

                });

            var eventName = @event.GetType().Name;
            eventName = ProcessEventName(eventName);

            consumerChannel.ExchangeDeclare(exchange: eventBusConfig.DefaultTopicName, "direct");

            var message = JsonConvert.SerializeObject(@event);
            var body = Encoding.UTF8.GetBytes(message);

            policy.Execute(() =>
            {
                var properties = consumerChannel.CreateBasicProperties();
                properties.DeliveryMode = 2 ; //persistence

               // consumerChannel.QueueDeclare(eventName,durable: true,exclusive: false,autoDelete: false,arguments: null); 
               // consumerChannel.QueueBind(queue: GetSubName(eventName), exchange: eventBusConfig.DefaultTopicName, routingKey: eventName);
               // controlde işe yarıyor.

                consumerChannel.BasicPublish(eventBusConfig.DefaultTopicName,eventName,mandatory:true,basicProperties:properties,body:body);
            });
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubsManager.HasSubscriptionsForEvent(eventName))
            {
                if (!rabbitMQPersistenceConnection.IsConnected)
                {
                    rabbitMQPersistenceConnection.TryConnect();
                }

                consumerChannel.QueueDeclare(queue: GetSubName(eventName),durable:true,exclusive:false,autoDelete:false,arguments:null); //Consume edilmişmi queue bakılır.

                consumerChannel.QueueBind(queue: GetSubName(eventName), exchange: eventBusConfig.DefaultTopicName, routingKey: eventName);

            }

            SubsManager.AddSubscription<T, TH>();

            StartBasicConsume(eventName);
        }

        public override void UnSubscribe<T, TH>()
        {
            SubsManager.RemoveSubscription<T, TH>(); 
        }

        private IModel CreateConsumerChannel()
        {
            if (!rabbitMQPersistenceConnection.IsConnected)
            {
                rabbitMQPersistenceConnection.TryConnect();
            }

            var channel = rabbitMQPersistenceConnection.CreatedModel();

            channel.ExchangeDeclare(exchange: eventBusConfig.DefaultTopicName,type: "direct");

            return channel;   
        }

        private void StartBasicConsume(string eventName)
        {
            if (consumerChannel != null)
            {
                var consumer = new EventingBasicConsumer(consumerChannel);

                consumer.Received += Consumer_Received;

                consumerChannel.BasicConsume(queue: GetSubName(eventName),autoAck:false,consumer:consumer);
            }
        }

        private async void Consumer_Received(object sender, BasicDeliverEventArgs e)
        {
            var eventName = e.RoutingKey;
            eventName = ProcessEventName(eventName);
            var message = Encoding.UTF8.GetString(e.Body.Span);

            try
            {
                await ProcessEvent(eventName,message);
            }
            catch 
            {

            }
            consumerChannel.BasicAck(e.DeliveryTag, false); 
        }
    }
}

