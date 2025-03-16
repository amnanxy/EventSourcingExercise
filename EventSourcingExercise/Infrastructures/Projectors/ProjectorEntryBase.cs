namespace EventSourcingExercise.Infrastructures.Projectors;

public abstract class ProjectorEntryBase
{
    public string TenantCode { get; internal set; } = null!;

    public long StreamId { get; internal set; }

    public int Version { get; internal set; }

    public DateTime CreatedAt { get; internal set; }
}