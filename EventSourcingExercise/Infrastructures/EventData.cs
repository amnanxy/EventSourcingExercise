namespace EventSourcingExercise.Infrastructures;

public record EventData(long StreamId, long Version, string EventText, string EventName, DateTimeOffset CreatedAt);