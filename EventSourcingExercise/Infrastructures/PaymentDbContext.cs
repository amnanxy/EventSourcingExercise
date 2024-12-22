﻿using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures;

public class PaymentDbContext : DbContext
{
    public PaymentDbContext(DbContextOptions<PaymentDbContext> options) : base(options)
    {
    }

    public DbSet<EventStream> EventStreams { get; init; }

    public DbSet<EventEntry> EventEntries { get; init; }

    public DbSet<StreamIdMapping> StreamIdMappings { get; init; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EventStream>(builder =>
        {
            builder.HasKey(t => t.Id)
                .HasName("pk_id");

            builder.Property(t => t.Id);

            builder.Property(t => t.AggregateRootTypeName)
                .HasMaxLength(30);

            builder.Property(t => t.Version)
                .IsConcurrencyToken();
        });

        modelBuilder.Entity<EventEntry>(builder =>
        {
            builder.HasKey(t => t.Id)
                .HasName("pk_id");
            
            builder.HasIndex(t => new { t.StreamId, t.Version })
                .IsUnique()
                .HasDatabaseName("uk_streamId_version");

            builder.Property(t => t.StreamId);

            builder.Property(t => t.Version);

            builder.Property(t => t.EventText)
                .HasMaxLength(500);

            builder.Property(t => t.EventName)
                .HasMaxLength(50);

            builder.Property(t => t.CreatedAt);

            builder.HasOne<EventStream>()
                .WithMany()
                .HasForeignKey(t => t.StreamId)
                .IsRequired();
        });

        modelBuilder.Entity<StreamIdMapping>(builder =>
        {
            builder.HasKey(t => t.AggregateId)
                .HasName("pk_aggregateId");

            builder.HasIndex(t => t.StreamId)
                .IsUnique()
                .HasDatabaseName("uk_streamId");

            builder.Property(t => t.AggregateId)
                .HasMaxLength(40);

            builder.Property(t => t.StreamId);
        });
    }
}