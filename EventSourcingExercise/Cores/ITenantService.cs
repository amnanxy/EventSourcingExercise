namespace EventSourcingExercise.Cores;

public interface ITenantService
{
    Task<string> GetTenantId();
}