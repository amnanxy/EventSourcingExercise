using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.PersistenceModels;

public class EventSourcingReadOnlyDbContext : EventSourcingDbContext
{
    public EventSourcingReadOnlyDbContext(DbContextOptions<EventSourcingReadOnlyDbContext> options)
        : base(options)
    {
    }
}