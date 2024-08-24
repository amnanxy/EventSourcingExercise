using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Transactions.Applications.UseCases.Captures;

public class CaptureCommand : IRequest<Result<CaptureResult?>>
{
    public required string TransactionId { get; init; }
}