using System.Text.Json;
using System.Text.Json.Serialization;
using Autofac.Extensions.DependencyInjection;
using EventSourcingExercise.Extensions;
using EventSourcingExercise.Modules.Generics.Entities;
using MediatR;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Host.UseServiceProviderFactory(new AutofacServiceProviderFactory());

builder.Services.AddSerilog((services, lc) => lc
    .ReadFrom.Configuration(builder.Configuration)
    .ReadFrom.Services(services)
    .Enrich.FromLogContext()
    .WriteTo.Console());

builder.Host.UseOrleans(static siloBuilder =>
{
    siloBuilder.UseLocalhostClustering()
        .AddMemoryStreams("StreamProvider")
        .AddMemoryGrainStorage("PubSubStore");
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.PropertyNamingPolicy = JsonNamingPolicy.CamelCase;
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter());
    });
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddHealthChecks();
builder.Services.AddTransient(typeof(IRequestHandler<,>), typeof(AggregateHandler<>));
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssemblyContaining<Program>());
builder.Services.AddServices();
builder.Services.AddEventTypeMapper();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/_hc");

app.Run();