using EventSourcingExercise.Transactions.Applications.UseCases.Captures;
using EventSourcingExercise.Utilities.Results;
using MediatR;

namespace EventSourcingExercise.Transactions.ApiModels.Captures;

public class CaptureRequest : IRequest<Result<CaptureResult?>>
{
    public required string TransactionId { get; init; }

    public CaptureCommand ToCommand()
    {
        return new CaptureCommand
        {
            TransactionId = TransactionId,
        };
    }
}