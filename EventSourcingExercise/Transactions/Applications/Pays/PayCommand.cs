using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Transactions.Applications.Pays;

public class PayCommand : IRequest<Result>
{
    public decimal Amount { get; init; }
}