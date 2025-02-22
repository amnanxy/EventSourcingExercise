using EventSourcingExercise.Infrastructures.EventSourcing.Models;
using MediatR;

namespace EventSourcingExercise.Infrastructures.EventSourcing;

public class EventsCreated : INotification
{
    public required string TenantCode { get; init; }

    public required IReadOnlyList<EventEntry> EventEntries { get; init; }

    public required IReadOnlyList<OutboxEntry> OutboxEntries { get; init; }
}