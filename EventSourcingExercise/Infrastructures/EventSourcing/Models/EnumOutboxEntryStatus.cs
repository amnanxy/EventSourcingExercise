namespace EventSourcingExercise.Infrastructures.EventSourcing.Models;

public enum EnumOutboxEntryStatus
{
    Waiting = 1,
    Delivered,
}