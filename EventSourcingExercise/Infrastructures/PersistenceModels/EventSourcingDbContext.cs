using EventSourcingExercise.Modules.Transactions.Domains.Enums;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.PersistenceModels;

public class EventSourcingDbContext : DbContext
{
    public EventSourcingDbContext(DbContextOptions<EventSourcingDbContext> options) : base(options)
    {
    }

    protected EventSourcingDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<EventStream> EventStreams { get; init; }

    public DbSet<EventEntry> EventEntries { get; init; }

    public DbSet<StreamIdMapping> StreamIdMappings { get; init; }

    public DbSet<OutboxEntry> OutboxEntries { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventStream>(builder =>
        {
            builder.ToTable("event_stream");

            builder.HasKey(t => t.Id)
                .HasName("pk_id");

            builder.Property(t => t.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.AggregateRootTypeName)
                .HasColumnName("aggregate_root_type_name")
                .HasColumnType("varchar(50)");

            builder.Property(t => t.Version)
                .HasColumnName("version")
                .HasColumnType("int")
                .IsConcurrencyToken();

            builder.Property(t => t.TenantId)
                .HasColumnName("tenant_id")
                .HasColumnType("varchar(40)");
        });

        modelBuilder.Entity<EventEntry>(builder =>
        {
            builder.ToTable("event_entry");

            builder.HasKey(t => t.Id)
                .HasName("pk_id");

            builder.HasIndex(t => new { t.StreamId, t.Version })
                .IsUnique()
                .HasDatabaseName("uk_stream_id_version");

            builder.Property(t => t.Id)
                .HasColumnName("id")
                .ValueGeneratedOnAdd();

            builder.Property(t => t.StreamId)
                .HasColumnName("stream_id")
                .HasColumnType("bigint");

            builder.Property(t => t.Version)
                .HasColumnName("version")
                .HasColumnType("int");

            builder.Property(t => t.EventText)
                .HasColumnName("event_text")
                .HasColumnType("jsonb");

            builder.Property(t => t.EventName)
                .HasColumnName("event_name")
                .HasColumnType("varchar(50)");

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime");

            builder.HasOne<EventStream>()
                .WithMany()
                .HasForeignKey(t => t.StreamId)
                .IsRequired();
        });

        modelBuilder.Entity<StreamIdMapping>(builder =>
        {
            builder.ToTable("stream_id_mapping");

            builder.HasKey(t => t.AggregateId)
                .HasName("pk_aggregate_id");

            builder.HasIndex(t => t.StreamId)
                .IsUnique()
                .HasDatabaseName("uk_stream_id");

            builder.Property(t => t.AggregateId)
                .HasColumnName("aggregate_id")
                .HasColumnType("varchar(40)");

            builder.Property(t => t.StreamId)
                .HasColumnName("stream_id")
                .HasColumnType("bigint");
        });

        modelBuilder.Entity<OutboxEntry>(builder =>
        {
            builder.ToTable("outbox_entry");

            builder.HasKey(t => t.EventId)
                .HasName("pk_event_id");

            builder.HasIndex(t => new { t.CreatedAt })
                .HasFilter($"status = '{EnumOutboxEntryStatus.Waiting}'")
                .HasDatabaseName("ix_created_at_filter");

            builder.Property(t => t.EventId)
                .HasColumnName("event_id");

            builder.Property(t => t.Status)
                .HasColumnName("status")
                .HasColumnType("varchar(40)");

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime");

            builder.Property(t => t.DeliveredAt)
                .HasColumnName("delivered_at")
                .HasColumnType("datetime")
                .IsRequired(false);
        });
    }

    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        configurationBuilder
            .Properties<EnumPaymentStatus>()
            .HaveConversion<string>();

        configurationBuilder
            .Properties<EnumCaptureStatus>()
            .HaveConversion<string>();

        configurationBuilder
            .Properties<EnumOutboxEntryStatus>()
            .HaveConversion<string>();
    }
}