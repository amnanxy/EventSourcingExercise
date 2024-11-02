using EventSourcingExercise.Infrastructures;
using EventSourcingExercise.Modules.Transactions.Domains;

namespace EventSourcingExercise.Extensions;

public static class EventTypeMapperExtensions
{
    public static IServiceCollection AddEventTypeMapper(this IServiceCollection services)
    {
        var typeMapper = new TypeMapper();
        typeMapper.AddAggregateRoot("Payment", typeof(Payment));
        typeMapper.AddEvent("NewPaymentStarted", typeof(PaymentEvents.NewPaymentStarted));
        typeMapper.AddEvent("PaymentSucceeded", typeof(PaymentEvents.PaymentSucceeded));
        typeMapper.AddEvent("PaymentFailed", typeof(PaymentEvents.PaymentFailed));
        typeMapper.AddEvent("CaptureAccepted", typeof(PaymentEvents.CaptureAccepted));

        services.AddSingleton(typeMapper);
        return services;
    }
}