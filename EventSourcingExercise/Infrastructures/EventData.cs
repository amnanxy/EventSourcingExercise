namespace EventSourcingExercise.Infrastructures;

public record EventData(long StreamId, long Version, string EventText, Type EventType, DateTimeOffset CreatedAt);