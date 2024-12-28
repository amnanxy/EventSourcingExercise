using EventSourcingExercise.Infrastructures.PersistenceModels;
using MediatR;

namespace EventSourcingExercise.Infrastructures;

public class EventsCreated : INotification
{
    public required IReadOnlyList<EventEntry> EventDataSet { get; init; }
}