using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.Payments;

public class PaymentReadonlyDbContext : PaymentDbContext
{
    public PaymentReadonlyDbContext(DbContextOptions options) : base(options)
    {
    }
}