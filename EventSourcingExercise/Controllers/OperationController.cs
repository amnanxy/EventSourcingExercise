using EventSourcingExercise.Infrastructures.Projectors;
using EventSourcingExercise.Infrastructures.Projectors.TransactionRecords;
using EventSourcingExercise.Modules.Generics.Entities;
using EventSourcingExercise.Modules.Transactions.Domains;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace EventSourcingExercise.Controllers;

[ApiController]
[Route("operation")]
public class OperationController : CommonControllerBase
{
    public OperationController(IMediator mediator) : base(mediator)
    {
    }

    [HttpGet]
    [Route("payment/{aggregateId}")]
    public async Task<object> GetPayment(
        [FromRoute] string aggregateId,
        CancellationToken token)
    {
        return await Mediator.Send(new AggregateQuery<Payment>
        {
            AggregateId = aggregateId,
        }, token);
    }

    [HttpGet]
    [Route("transaction-record/{aggregateId}")]
    public async Task<object?> GetTransactionRecord(
        [FromRoute] string aggregateId,
        [FromServices] ProjectorDbContext projectorDbContext,
        CancellationToken token)
    {
        return await projectorDbContext.TransactionRecords
            .SingleOrDefaultAsync(t => t.PaymentId == aggregateId, cancellationToken: token);
    }
}