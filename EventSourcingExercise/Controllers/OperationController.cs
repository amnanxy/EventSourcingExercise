using EventSourcingExercise.Modules.Generics.Entities;
using EventSourcingExercise.Modules.Transactions.Domains;
using MediatR;
using Microsoft.AspNetCore.Mvc;

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
        } , token);
    }
}