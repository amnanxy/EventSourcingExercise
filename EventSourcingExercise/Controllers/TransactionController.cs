using EventSourcingExercise.Transactions.ApiModels.Pays;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingExercise.Controllers;

[ApiController]
[Route("transaction")]
public class TransactionController : ControllerBase
{
    private readonly IMediator _mediator;

    public TransactionController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpPost]
    [Route("pay")]
    public async Task<object> Pay(
        [FromBody] PayRequest request,
        CancellationToken token)
    {
        var command = request.ToCommand();
        return await _mediator.Send(command, token);
    }
}