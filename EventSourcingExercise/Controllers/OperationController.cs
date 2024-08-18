using EventSourcingExercise.Generics.Entities;
using EventSourcingExercise.Transactions.Domains;
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
    [Route("payment/{entityId}")]
    public async Task<object> GetEntity(
        [FromRoute] string entityId,
        CancellationToken token)
    {
        return await Mediator.Send(new EntityQuery<Payment>
        {
            EntityId = entityId,
        } , token);
    }
}