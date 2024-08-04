using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Transactions.Applications.Pays;

public class PayCommand : IRequest<Result<PayResult?>>
{
    public decimal Amount { get; init; }
}