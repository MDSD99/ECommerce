using System;
using System.Text;
using EventBus.Base;
using EventBus.Base.Events;
using Microsoft.Azure.ServiceBus;
using Microsoft.Azure.ServiceBus.Management;
using Newtonsoft.Json;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

#nullable disable
namespace EventBus.AzureServiceBus
{
    public class EventBusServiceBus : BaseEventBus
    {
        private ITopicClient topicClient;

        private ManagementClient managementClient;

        private ILogger logger;

        public EventBusServiceBus(IServiceProvider serviceProvider, EventBusConfig config) : base(serviceProvider, config)
        {
            logger = serviceProvider.GetService(typeof(ILogger<EventBusServiceBus>)) as ILogger<EventBusServiceBus>; 

            managementClient = new ManagementClient(config.EventBusConnectionString);

            topicClient = CreateTopicClient();
        }

        private ITopicClient CreateTopicClient()
        {
            if(topicClient==null || topicClient.IsClosedOrClosing)
            {
                topicClient = new TopicClient(eventBusConfig.EventBusConnectionString,eventBusConfig.DefaultTopicName,RetryPolicy.Default);
            }
               
            if (managementClient.TopicExistsAsync(eventBusConfig.DefaultTopicName).GetAwaiter().GetResult())
                managementClient.CreateTopicAsync(eventBusConfig.DefaultTopicName).GetAwaiter().GetResult();

            return topicClient;
        }

        public override void Publish(IntegrationEvent @event)
        {
            var eventName = @event.GetType().Name; //OrderCreatedIntegrationEvent

            eventName = ProcessEventName(eventName);//OrderCreated

            string eventStr= JsonConvert.SerializeObject(@event);

            byte[] bodyArr = Encoding.UTF8.GetBytes(eventStr);

            var message = new Message()
            {
                MessageId = Guid.NewGuid().ToString(),
                Body = bodyArr,
                Label = eventName,
            };

            topicClient.SendAsync(message).GetAwaiter().GetResult();
        }

        public override void Subscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            eventName = ProcessEventName(eventName);

            if (!SubsManager.HasSubscriptionsForEvent(eventName))
            {
                var subscriptionClient=CreateSubscriptionClientIfNotExists(eventName);

                RegisterSubscriptionClientMessageHandler(subscriptionClient);

            }

            logger.LogInformation("Subscripe to event : {EventName} with {EventHandler}", eventName, typeof(TH).Name);

            SubsManager.AddSubscription<T, TH>();
        }

        public override void UnSubscribe<T, TH>()
        {
            var eventName = typeof(T).Name;
            try
            {
                var subscriptionClient = CreateSubscriptionClient(eventName);

                subscriptionClient.RemoveRuleAsync(eventName).GetAwaiter().GetResult();
            }
            catch //(Exception ex)
            {
                logger.LogWarning("The messaging entity {EventName} Could not be found.", eventName);
            }

            logger.LogInformation("Unsubscripe from event {EventName}", eventName);

            SubsManager.RemoveSubscription<T, TH>();
        }

        private void RegisterSubscriptionClientMessageHandler(ISubscriptionClient subscriptionClient)
        {
            subscriptionClient.RegisterMessageHandler(
                async (message, token) =>
                {
                    var eventName = $"{message.Label}";
                    var messageData = Encoding.UTF8.GetString(message.Body);

                    if (await ProcessEvent(ProcessEventName(eventName), messageData))
                    {
                        await subscriptionClient.CompleteAsync(message.SystemProperties.LockToken);
                    }
                },
                new MessageHandlerOptions(ExceptionReceivedHandler) { MaxConcurrentCalls = 10,AutoComplete=false }
                ) ;
        }

        private Task ExceptionReceivedHandler(ExceptionReceivedEventArgs exceptionReceivedEventArgs)
        {
            var ex = exceptionReceivedEventArgs.Exception;
            var context = exceptionReceivedEventArgs.ExceptionReceivedContext;

            logger.LogError(ex,"ERROR handling message : {ExceptionMessage} - Context : {@ExceptionContext}",ex.Message,context);

            return Task.CompletedTask;
        }

        private ISubscriptionClient CreateSubscriptionClientIfNotExists(string eventName)
        {
            var subClient = CreateSubscriptionClient(eventName);

            var exists = managementClient.SubscriptionExistsAsync(eventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();

            if (!exists)
            {
                managementClient.CreateSubscriptionAsync(eventBusConfig.DefaultTopicName, GetSubName(eventName)).GetAwaiter().GetResult();
                RemoveDefaultRule(subClient);
            }

            CreateRuleIfNotExists(eventName, subClient);

            return subClient;
        }

        private void CreateRuleIfNotExists(string eventName,ISubscriptionClient subscriptionClient)
        {
            bool ruleExists;

            try
            {

                var rule = managementClient.GetRuleAsync(eventBusConfig.DefaultTopicName, eventName, eventName).GetAwaiter().GetResult();
                
                ruleExists = rule != null;

            }
            catch (MessagingEntityNotFoundException)
            {
                ruleExists = false;
            }

            if (!ruleExists)
            {
                subscriptionClient.AddRuleAsync(new RuleDescription
                {
                    Filter = new CorrelationFilter { Label = eventName },
                    Name = eventName
                });
            }
        }

        private void RemoveDefaultRule(SubscriptionClient subscriptionClient)
        {
            try
            {
                subscriptionClient
                    .RemoveRuleAsync(RuleDescription.DefaultRuleName)
                    .GetAwaiter()
                    .GetResult();
            }
            catch (MessagingEntityNotFoundException)
            {
                logger.LogWarning("The messaging entity {DefaultRuleName} could not be found.",RuleDescription.DefaultRuleName);
            }
        }

        private SubscriptionClient CreateSubscriptionClient(string eventName)
            => new SubscriptionClient(eventBusConfig.EventBusConnectionString,eventBusConfig.DefaultTopicName,GetSubName(eventName));

        public override void Dispose()
        {
            base.Dispose();
            topicClient.CloseAsync().GetAwaiter().GetResult();
            managementClient.CloseAsync().GetAwaiter().GetResult();
            managementClient = null;
            topicClient = null;
        }
    }
}

