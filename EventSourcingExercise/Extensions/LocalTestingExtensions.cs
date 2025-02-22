using EventSourcingExercise.Infrastructures.EventSourcing;
using EventSourcingExercise.Infrastructures.Payments;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Extensions;

public static class LocalTestingExtensions
{
    private const string EventSourcing = "ES";
    private const string Payment = "Payment";
    private static readonly SqliteConnection EventSourcingConnection;
    private static readonly SqliteConnection PaymentConnection;

    static LocalTestingExtensions()
    {
        EventSourcingConnection = new SqliteConnection("Filename=:memory:");
        PaymentConnection = new SqliteConnection("Filename=:memory:");
    }

    public static IServiceCollection UseMemoryDbContext(this IServiceCollection services)
    {
        EventSourcingConnection.Open();
        PaymentConnection.Open();
        return services.AddSingleton<Func<string, SqliteConnection>>(_ => (dbName) =>
            {
                return dbName switch
                {
                    EventSourcing => EventSourcingConnection,
                    Payment => PaymentConnection,
                    _ => throw new NotSupportedException(),
                };
            })
            .AddDbContextFactory<EventSourcingDbContext>((provider, options) =>
            {
                var factory = provider.GetRequiredService<Func<string, SqliteConnection>>();
                options.UseSqlite(factory(EventSourcing));
            }, ServiceLifetime.Scoped)
            .AddDbContextFactory<EventSourcingReadOnlyDbContext>((provider, options) =>
            {
                var factory = provider.GetRequiredService<Func<string, SqliteConnection>>();
                options.UseSqlite(factory(EventSourcing))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Scoped)
            .AddDbContextFactory<PaymentDbContext>((provider, options) =>
            {
                var factory = provider.GetRequiredService<Func<string, SqliteConnection>>();
                options.UseSqlite(factory(Payment));
            }, ServiceLifetime.Scoped)
            .AddDbContextFactory<PaymentReadonlyDbContext>((provider, options) =>
            {
                var factory = provider.GetRequiredService<Func<string, SqliteConnection>>();
                options.UseSqlite(factory(Payment))
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            }, ServiceLifetime.Scoped);
    }
}