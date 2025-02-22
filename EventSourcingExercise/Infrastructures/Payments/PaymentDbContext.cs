using EventSourcingExercise.Infrastructures.EventSourcing.Models;
using EventSourcingExercise.Modules.Transactions.Applications.Models;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.Payments;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    protected PaymentDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<StreamIdMapping> StreamIdMappings { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        modelBuilder.Entity<StreamIdMapping>(builder =>
        {
            builder.ToTable("stream_id_mapping");

            builder.HasKey(t => t.AggregateCode)
                .HasName("pk_aggregate_code");

            builder.HasIndex(t => t.StreamId)
                .IsUnique()
                .HasDatabaseName("uk_stream_id");

            builder.Property(t => t.AggregateCode)
                .HasColumnName("aggregate_code")
                .HasColumnType("varchar(40)");

            builder.Property(t => t.StreamId)
                .HasColumnName("stream_id")
                .HasColumnType("bigint");
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<EnumOutboxEntryStatus>()
            .HaveConversion<string>();
    }
}