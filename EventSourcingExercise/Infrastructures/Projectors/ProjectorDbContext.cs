using EventSourcingExercise.Infrastructures.Projectors.TransactionRecords;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.Projectors;

public class ProjectorDbContext : DbContext
{
    public ProjectorDbContext(DbContextOptions<ProjectorDbContext> options) : base(options)
    {
    }

    protected ProjectorDbContext(DbContextOptions options) : base(options)
    {
    }

    public DbSet<TransactionRecord> TransactionRecords { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<TransactionRecord>(builder =>
        {
            builder.ToTable("transaction_record");

            builder.HasKey(t => t.StreamId)
                .HasName("pk_stream_id");

            builder.Property(t => t.StreamId)
                .HasColumnType("long")
                .HasColumnName("stream_id")
                .ValueGeneratedNever();

            builder.Property(t => t.TenantCode)
                .HasColumnName("tenant_code")
                .HasColumnType("varchar(40)");

            builder.Property(t => t.Version)
                .HasColumnName("version")
                .HasColumnType("int");

            builder.Property(t => t.CreatedAt)
                .HasColumnName("created_at")
                .HasColumnType("datetime");

            builder.HasIndex(t => t.PaymentId)
                .IsUnique()
                .HasDatabaseName("uk_payment_id");

            builder.Property(t => t.PaymentId)
                .HasColumnName("payment_id")
                .HasColumnType("varchar(40)");

            builder.Property(t => t.Amount)
                .HasColumnName("amount")
                .HasColumnType("decimal(19,4)")
                .HasPrecision(19, 4);
            
            builder.Property(t => t.Status)
                .HasColumnName("status")
                .HasColumnType("varchar(40)");
        });
    }
}