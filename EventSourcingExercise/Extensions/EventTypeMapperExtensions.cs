using EventSourcingExercise.Infrastructures;
using EventSourcingExercise.Modules.Transactions.Domains;

namespace EventSourcingExercise.Extensions;

public static class EventTypeMapperExtensions
{
    public static IServiceCollection AddEventTypeMapper(this IServiceCollection services)
    {
        var eventTypeMapper = new EventTypeMapper();
        eventTypeMapper.Add("NewPaymentStarted", typeof(PaymentEvents.NewPaymentStarted));
        eventTypeMapper.Add("PaymentSucceeded", typeof(PaymentEvents.PaymentSucceeded));
        eventTypeMapper.Add("PaymentFailed", typeof(PaymentEvents.PaymentFailed));
        eventTypeMapper.Add("CaptureAccepted", typeof(PaymentEvents.CaptureAccepted));

        services.AddSingleton(eventTypeMapper);
        return services;
    }
}