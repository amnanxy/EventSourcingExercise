using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Transactions.Applications.UseCases.Pays;

public class PayCommand : IRequest<Result<PayResult?>>
{
    public decimal Amount { get; init; }
}