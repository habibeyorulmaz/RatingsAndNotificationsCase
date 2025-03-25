using Asp.Versioning;
using MassTransit;
using Microsoft.EntityFrameworkCore;
using Notifications.Api.Data.Context;
using Notifications.Api.Repositories;
using Notifications.Api.Services;
using SharedLibrary.Middlewares.GlobalExceptionHandler;
using System.Reflection;

var builder = WebApplication.CreateBuilder(args);

// Api Versioning
builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new ApiVersion(1);
    options.ReportApiVersions = true;
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ApiVersionReader = new UrlSegmentApiVersionReader();
}).AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'V";
    options.SubstituteApiVersionInUrl = true;
});
// Add services to the container.
// Buraya yazýlacak
builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// DbContext
builder.Services.AddDbContext<AppNotificationsDbContext>(options =>
    options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("InMemoryNotificationsDb"))

    );

// Logging
builder.Logging.ClearProviders().AddConsole();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();
builder.Services.AddScoped<INotificationService, NotificationService>();
// Masstransit
builder.Services.AddMassTransit(busConfigurator =>
{
    busConfigurator.SetKebabCaseEndpointNameFormatter();
    var entryAssembly = Assembly.GetExecutingAssembly();
    busConfigurator.AddConsumers(entryAssembly);
    busConfigurator.UsingRabbitMq((context, busFactoryConfigurator) =>
    {
        //busFactoryConfigurator.Host("localhost", "/", h => { });
        busFactoryConfigurator.Host("rabbit-test", "/", h =>
        {
            h.Username("guest");
            h.Password("guest");
        });

        busFactoryConfigurator.UseConcurrencyLimit(1);
        busFactoryConfigurator.UseMessageRetry(c =>
        {
            c.Handle<DbUpdateConcurrencyException>();
            c.Incremental(5,
                TimeSpan.FromSeconds(1),
                TimeSpan.Zero);
        });

        busFactoryConfigurator.ConfigureEndpoints(context);
    });
});

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseRouting();
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

app.Run();