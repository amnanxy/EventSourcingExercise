using EventSourcingExercise.Infrastructures.PersistenceModels;
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
        }).AddDbContextFactory<PaymentDbContext>((provider, options) =>
        {
            var connection = provider.GetRequiredService<SqliteConnection>();
            options.UseSqlite(connection);
        }, ServiceLifetime.Scoped);
    }
}