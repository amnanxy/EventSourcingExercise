﻿using MediatR;

namespace EventSourcingExercise.Infrastructures;

public class EventsCreated : INotification
{
    public required IReadOnlyList<EventData> EventDataSet { get; init; }
}