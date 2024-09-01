using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Modules.Transactions.Applications.UseCases.Captures;

public class CaptureCommand : IRequest<Result<CaptureResult?>>
{
    public required string TransactionId { get; init; }
}