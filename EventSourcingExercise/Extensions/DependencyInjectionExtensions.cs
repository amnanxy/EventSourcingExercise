using EventSourcingExercise.Transactions.Applications;
using EventSourcingExercise.Transactions.Applications.ThirdPartyGateways;
using EventSourcingExercise.Transactions.Infrastructures;
using EventSourcingExercise.Utilities.IdGenerators;

namespace EventSourcingExercise.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IUnitOfWork, UnitOfWork>();
        services.AddScoped<IPaymentRepository, PaymentRepository>();
        services.AddScoped<IThirdPartyGateway, ThirdPartyGateway>();
        services.AddScoped<IIdGenerator<long>>(_ => new NumberIdGenerator(0));

        return services;
    }
}