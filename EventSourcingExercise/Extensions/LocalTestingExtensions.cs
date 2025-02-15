using EventSourcingExercise.Infrastructures.EventSourcing;
using EventSourcingExercise.Infrastructures.EventSourcing.Models;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Extensions;

public static class LocalTestingExtensions
{
    public static IServiceCollection UseMemoryDbContext(this IServiceCollection services)
    {
        return services.AddSingleton(_ =>
                {
                    var connection = new SqliteConnection("Filename=:memory:");
                    connection.Open();
                    return connection;
                })
                .AddDbContextFactory<EventSourcingDbContext>((provider, options) =>
                {
                    var connection = provider.GetRequiredService<SqliteConnection>();
                    options.UseSqlite(connection);
                }, ServiceLifetime.Scoped)
                .AddDbContextFactory<EventSourcingReadOnlyDbContext>((provider, options) =>
                {
                    var connection = provider.GetRequiredService<SqliteConnection>();
                    options.UseSqlite(connection)
                        .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
                }, ServiceLifetime.Scoped);
    }
}