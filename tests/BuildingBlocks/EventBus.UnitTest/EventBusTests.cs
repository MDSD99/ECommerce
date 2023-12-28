using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using EventBus.UnitTest.Events;
using EventBus.UnitTest.Events.EventHandler;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace EventBus.UnitTest;

[TestClass]
public class EventBusTests
{
    private ServiceCollection services;

    public EventBusTests()
    {
        this.services= new ServiceCollection();
        services.AddLogging(configure=> configure.AddConsole());

    }

    [TestMethod]
    public void SubscribeEventOnRabbitMQTest()
    {
        services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetRabbitMQConfig(), sp);
        });

        var sp = services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Subscribe<OrderCreatedIntegrationEvent,OrderCratedIntegrationEventHandler>();
        //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCratedIntegrationEventHandler>();
    }

    [TestMethod]
    public void SubscribeEventOnAzureTest()
    {
        services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetAzureConfig(), sp);
        });

        var sp = services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Subscribe<OrderCreatedIntegrationEvent, OrderCratedIntegrationEventHandler>();
        //eventBus.UnSubscribe<OrderCreatedIntegrationEvent, OrderCratedIntegrationEventHandler>();
    }

    [TestMethod]
    public void SendMessageRabbitMQTest()
    {
        services.AddSingleton<IEventBus>(sp=>
        {
            return EventBusFactory.Create(GetRabbitMQConfig(), sp);
        });

        var sp = services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    [TestMethod]
    public void SendMessageAzureTest()
    {
        services.AddSingleton<IEventBus>(sp =>
        {
            return EventBusFactory.Create(GetAzureConfig(), sp);
        });

        var sp = services.BuildServiceProvider();

        var eventBus = sp.GetRequiredService<IEventBus>();

        eventBus.Publish(new OrderCreatedIntegrationEvent(1));
    }

    private EventBusConfig GetAzureConfig()=>
    new (){
                ConnectionRetryCount = 5,
                SubscriberClientAppName = "EventBus.UnitTest",
                DefaultTopicName = "MehmetDefaultTopic",
                EventBusType = EventBusType.AzureServiceBus,
                EventNameSuffix = "IntegrationEvent",
                EventNamePrefix = "",
                EventBusConnectionString="AzureBusConnectionString"
                //Connection = new ConnectionFactory
                //{
                //    HostName = "localhost",
                //    Port = 15672,
                //    UserName = "guest",
                //    Password = "guest"
                //}
     };
    private EventBusConfig GetRabbitMQConfig()=>
    new()
    {
        ConnectionRetryCount = 5,
        SubscriberClientAppName = "EventBus.UnitTest",
        DefaultTopicName = "MehmetDefaultTopic",
        EventBusType = EventBusType.RabbitMQ,
        EventNameSuffix = "IntegrationEvent",
        EventNamePrefix = ""
        //Connection = new ConnectionFactory
        //{
        //    HostName = "localhost",
        //    Port = 15672,
        //    UserName = "guest",
        //    Password = "guest"
        //}
    };
}
