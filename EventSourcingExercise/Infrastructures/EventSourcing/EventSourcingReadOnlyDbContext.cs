using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.EventSourcing;

public class EventSourcingReadOnlyDbContext : EventSourcingDbContext
{
    public EventSourcingReadOnlyDbContext(DbContextOptions<EventSourcingReadOnlyDbContext> options)
        : base(options)
    {
    }
}