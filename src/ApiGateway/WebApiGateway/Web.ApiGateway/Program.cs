
using Ocelot.DependencyInjection;
using Ocelot.Middleware;
using Ocelot.Provider.Consul;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost//.ConfigureServices(s=>s.AddSingleton(builder))
//    .ConfigureAppConfiguration((hostingContext, config) =>
//{
//    config.SetBasePath(hostingContext.HostingEnvironment.ContentRootPath)
//          .AddJsonFile("Configurations/ocelot.json")
//          .AddEnvironmentVariables();
//});---------------


//IConfigurationBuilder obuild = new ConfigurationBuilder()
//    .SetBasePath(builder.Environment.ContentRootPath)
//    .AddJsonFile("Configurations/ocelot.json")
//    .AddEnvironmentVariables();


builder.Configuration
    .SetBasePath(builder.Environment.ContentRootPath)
    .AddJsonFile("ocelot.json")
    .AddEnvironmentVariables();
builder.Services.AddOcelot().AddConsul();
// Add services to the container.

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

app.UseAuthorization();

app.MapControllers();

app.UseOcelot().Wait();

app.Run();

