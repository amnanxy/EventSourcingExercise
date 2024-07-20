using EventSourcingExercise.Transactions.Applications;
using EventSourcingExercise.Transactions.Infrastructures;

namespace EventSourcingExercise.Extensions;

public static class DependencyInjectionExtensions
{
    public static IServiceCollection AddServices(this IServiceCollection services)
    {
        services.AddScoped<IPaymentRepository, PaymentRepository>();

        return services;
    }
}