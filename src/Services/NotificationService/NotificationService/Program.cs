using Microsoft.Extensions.DependencyInjection;
using EventBus.Base;
using EventBus.Factory;
using EventBus.Base.Abstraction;
using Microsoft.Extensions.Logging;
using NotificationService.IntegrationEvents.EventHandlers;
using NotificationService.IntegrationEvents.Events;

ServiceCollection services = new ServiceCollection();

services.AddLogging(configure =>
{
    configure.AddConsole();
});

services.AddTransient<OrderPaymentFailedIntegrationEventHandler>();
services.AddTransient<OrderPaymentSuccessIntegrationEventHandler>();

services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "NotificationService",
        EventBusType = EventBusType.RabbitMQ
    };
    return EventBusFactory.Create(config, sp);
});

services.BuildServiceProvider().GetRequiredService<IEventBus>().Subscribe<OrderPaymentSuccessIntegrationEvent,OrderPaymentSuccessIntegrationEventHandler>();
services.BuildServiceProvider().GetRequiredService<IEventBus>().Subscribe<OrderPaymentFailedIntegrationEvent, OrderPaymentFailedIntegrationEventHandler>();


Console.WriteLine("Application is running.");
Console.ReadLine();