using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.EventSourcing.Models;

public class EventSourcingReadOnlyDbContext : EventSourcingDbContext
{
    public EventSourcingReadOnlyDbContext(DbContextOptions<EventSourcingReadOnlyDbContext> options)
        : base(options)
    {
    }
}