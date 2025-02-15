namespace EventSourcingExercise.Infrastructures.EventSourcing.Models;

public class OutboxEntry
{
    public long EventId { get; init; }

    public EnumOutboxEntryStatus Status { get; set; }

    public DateTime CreatedAt { get; init; }

    public DateTime? DeliveredAt { get; set; }
}