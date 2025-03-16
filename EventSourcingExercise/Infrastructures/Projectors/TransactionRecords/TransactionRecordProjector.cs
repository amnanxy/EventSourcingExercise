using EventSourcingExercise.Modules.Transactions.Domains;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Infrastructures.Projectors.TransactionRecords;

[ImplicitStreamSubscription(ProjectorName.TransactionRecord)]
public class TransactionRecordProjector : ProjectorBase<TransactionRecord>
{
    public TransactionRecordProjector(
        ILogger<TransactionRecordProjector> logger,
        TypeMapper typeMapper,
        IDbContextFactory<ProjectorDbContext> dbContextFactory)
        : base(logger, typeMapper, dbContextFactory)
    {
        Create<PaymentEvents.NewPaymentStarted>(eventData =>
        {
            var evt = eventData.Event;
            return new TransactionRecord
            {
                PaymentId = evt.PaymentId,
                Amount = evt.Amount,
                Status = EnumTransactionStatus.PaymentPending,
            };
        });

        Apply<PaymentEvents.PaymentSucceeded>((data, record) => record.Status = EnumTransactionStatus.PaymentSuccess);
    }
}