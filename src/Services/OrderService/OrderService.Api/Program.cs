using OrderService.Api.Extensions;
using OrderService.Infrastructure.Context;
using OrderService.Application;
using OrderService.Infrastructure;
using OrderService.Api.Extensions.EventHandlerRegistration;
using OrderService.Api.Extensions.ServiceDiscovery;
using EventBus.Base;
using EventBus.Factory;
using EventBus.Base.Abstraction;
using OrderService.Api.IntegrationEvents.Events;
using OrderService.Api.IntegrationEvents.EventHandlers;

var builder = WebApplication.CreateBuilder(args);


builder.WebHost.UseDefaultServiceProvider((context, options) =>
{
    options.ValidateOnBuild = false;

}).ConfigureAppConfiguration(i=>i.AddConfiguration(builder.Configuration));

builder.Configuration.SetBasePath(Directory.GetCurrentDirectory()).AddJsonFile("appsettings.json",false,true).AddEnvironmentVariables();


builder.Services.MigrateDbContext<OrderDbContext>((context,services) =>
{
    var logger = services.GetService<ILogger<OrderDbContext>>();

    var dbContextSeeder =new OrderDbContextSeed();
    dbContextSeeder.SeedAsync(context, logger).Wait();
});

builder.Services.AddLogging(configure =>
{
    configure.AddConsole();
})
.AddApplicationRegistration()
.AddPersistenceRegistration(builder.Configuration)
.ConfigureEventHandlers()
.AddServiceDiscoveryRegistration(builder.Configuration);

builder.Services.AddSingleton(sp =>
{
    EventBusConfig eventBusConfig = new()
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "OrderService",
        //Connection = new ConnectionFactory(),
        EventBusType = EventBusType.RabbitMQ    
    };

    return EventBusFactory.Create(eventBusConfig,sp);
});

builder.Services.AddControllers();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();


if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Services.GetRequiredService<IEventBus>().Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();

