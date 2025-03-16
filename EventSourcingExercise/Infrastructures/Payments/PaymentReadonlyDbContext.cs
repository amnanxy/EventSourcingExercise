using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.Payments;

public class PaymentReadonlyDbContext : PaymentDbContext
{
    public PaymentReadonlyDbContext(DbContextOptions<PaymentReadonlyDbContext> options) : base(options)
    {
    }
}