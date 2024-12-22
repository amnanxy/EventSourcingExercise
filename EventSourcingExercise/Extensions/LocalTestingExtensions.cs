using EventSourcingExercise.Infrastructures;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Extensions;

public static class LocalTestingExtensions
{
    public static IServiceCollection UseMemoryDbContext(this IServiceCollection services)
    {
        services.AddSingleton(_ =>
        {
            var connection = new SqliteConnection("Filename=:memory:");
            connection.Open();
            return connection;
        });

        services.AddScoped<PaymentDbContext>(provider =>
        {
            var connection = provider.GetRequiredService<SqliteConnection>();
            var options = new DbContextOptionsBuilder<PaymentDbContext>()
                .UseSqlite(connection)
                .Options;
            var dbContext = new PaymentDbContext(options);
            dbContext.Database.EnsureCreated();
            return dbContext;
        });

        return services;
    }
}