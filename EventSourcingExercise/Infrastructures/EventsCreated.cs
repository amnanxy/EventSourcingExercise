using EventSourcingExercise.Infrastructures.PersistenceModels;
using MediatR;

namespace EventSourcingExercise.Infrastructures;

public class EventsCreated : INotification
{
    public required string TenantId { get; init; }

    public required IReadOnlyList<EventEntry> EventEntries { get; init; }

    public required IReadOnlyList<OutboxEntry> OutboxEntries { get; init; }
}