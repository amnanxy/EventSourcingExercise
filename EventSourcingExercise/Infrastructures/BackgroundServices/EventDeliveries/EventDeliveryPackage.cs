using EventSourcingExercise.Infrastructures.PersistenceModels;

namespace EventSourcingExercise.Infrastructures.BackgroundServices.EventDeliveries;

public class EventDeliveryPackage
{
    public required string TenantId { get; init; }

    public required IReadOnlyList<EventEntry> EventEntries { get; init; }

    public required IReadOnlyList<OutboxEntry> OutboxEntries { get; init; }
}