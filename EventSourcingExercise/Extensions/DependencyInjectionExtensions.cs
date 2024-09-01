using EventSourcingExercise.Cores;
using EventSourcingExercise.Infrastructures;
using EventSourcingExercise.Infrastructures.IdGenerators;
using EventSourcingExercise.Modules.Transactions.Applications.ThirdPartyGateways;
using EventSourcingExercise.Utilities.IdGenerators;

namespace EventSourcingExercise.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<TimeProvider>(_ => TimeProvider.System);
        services.AddScoped<AggregateStoreBase, MemoryAggregateStore>();
        services.AddScoped<IThirdPartyGateway, ThirdPartyGateway>();
        services.AddScoped<INumberIdGenerator>(_ => new NumberIdGenerator(0));
        services.AddScoped<ITextIdGenerator, TextIdGenerator>();

        return services;
    }
}