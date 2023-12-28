using BasketService.Api.Core.Application.Repository;
using BasketService.Api.Core.Application.Services;
using BasketService.Api.Extensions;
using BasketService.Api.Infrastructure.Repository;
using BasketService.Api.IntegrationEvents.EventHandlers;
using BasketService.Api.IntegrationEvents.Events;
using EventBus.Base;
using EventBus.Base.Abstraction;
using EventBus.Factory;
using RabbitMQ.Client;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.ConfigureAuth(builder.Configuration);

builder.Services.AddSingleton(sp => sp.ConfigureRedis(builder.Configuration));

builder.Services.ConfigureConsul(builder.Configuration);

builder.Services.AddHttpContextAccessor();

builder.Services.AddScoped<IBasketRepository, RedisBasketRepository>();

builder.Services.AddTransient<IIdentityService, IdentityService>();

builder.Services.AddTransient<OrderCreatedIntegrationEventHandler>();

builder.Services.AddSingleton<IEventBus>(sp =>
{
    EventBusConfig config = new EventBusConfig
    {
        ConnectionRetryCount = 5,
        EventNameSuffix = "IntegrationEvent",
        SubscriberClientAppName = "BasketService.Api",
        Connection = new ConnectionFactory(),
        EventBusType = EventBusType.RabbitMQ
    };
    return EventBusFactory.Create(config, sp);
});

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRouting();//Gerekli olmayabilir.

app.UseAuthentication();

app.UseAuthorization();

app.MapControllers();

app.RegisterWithConsul(app.Lifetime);

app.Services.GetRequiredService<IEventBus>().Subscribe<OrderCreatedIntegrationEvent, OrderCreatedIntegrationEventHandler>();

app.Run();

