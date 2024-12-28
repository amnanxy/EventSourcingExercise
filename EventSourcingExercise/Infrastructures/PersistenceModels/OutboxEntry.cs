namespace EventSourcingExercise.Infrastructures.PersistenceModels;

public class OutboxEntry
{
    public long EventId { get; set; }

    public EnumOutboxEntryStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset? DeliveredAt { get; set; }
}