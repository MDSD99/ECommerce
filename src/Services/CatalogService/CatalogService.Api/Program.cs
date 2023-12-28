using CatalogService.Api.Extensions;
using CatalogService.Api.Infrastructure;
using CatalogService.Api.Infrastructure.Context;
using CatalogService.Api.Infrastructure.Extensions;

var builder = WebApplication.CreateBuilder(args);

//builder.WebHost.UseWebRoot("Pics").UseContentRoot(Directory.GetCurrentDirectory());

builder.WebHost.UseContentRoot(Directory.GetCurrentDirectory());

builder.Services.Configure<CatalogSettings>(builder.Configuration.GetSection("CatalogSettings"));

builder.Services.ConfigureDbContext(builder.Configuration);


builder.Services.MigrateDbContext<CatalogContext>((context, services) =>
{
    var env = services.GetService<IWebHostEnvironment>();

    var logger = services.GetService<ILogger<CatalogContextSeed>>();

    new CatalogContextSeed().SeedAsync(context,env,logger).Wait();

});

builder.Services.ConfigureConsul(builder.Configuration);

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

app.UseStaticFiles();
app.UseDefaultFiles();

app.UseAuthorization();

app.MapControllers();

app.RegisterWithConsul(app.Lifetime);

app.Run();

