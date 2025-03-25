using Asp.Versioning;
using FluentValidation;
using FluentValidation.AspNetCore;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Ratings.Api.Data.Context;
using Ratings.Api.Repositories;
using Ratings.Api.Services;
using SharedLibrary.Filters;
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

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(Assembly.GetExecutingAssembly());
builder.Services.AddControllers();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// FluentValidation
builder.Services.Configure<ApiBehaviorOptions>(options =>
{
    options.SuppressModelStateInvalidFilter = true;
});
builder.Services.AddMvc(options =>
{
    // Add our custom validation filter to handle validation errors
    options.Filters.Add<CustomValidationFilter>();
});
builder.Services.AddFluentValidationAutoValidation()
            .AddFluentValidationClientsideAdapters()
            .AddValidatorsFromAssemblyContaining<Program>();

// DbContext
builder.Services.AddDbContext<AppRatingsDbContext>(options =>
    options.UseInMemoryDatabase(builder.Configuration.GetConnectionString("InMemoryRatingsDb"))

    );

builder.Services.AddScoped<IRatingRepository, RatingRepository>();
builder.Services.AddScoped<IRatingService, RatingService>();

// Logging
builder.Logging.ClearProviders().AddConsole();

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
app.UseMiddleware<GlobalExceptionHandlerMiddleware>();

app.UseHttpsRedirection();
app.MapControllers();

app.Run();