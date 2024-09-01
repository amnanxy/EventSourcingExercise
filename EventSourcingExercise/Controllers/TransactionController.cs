using EventSourcingExercise.ApiModels.Transactions.Captures;
using EventSourcingExercise.ApiModels.Transactions.Pays;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace EventSourcingExercise.Controllers;

[ApiController]
[Route("transaction")]
public class TransactionController : CommonControllerBase
{
    public TransactionController(IMediator mediator) : base(mediator)
    {
    }

    [HttpPost]
    [Route("pay")]
    public async Task<object> Pay(
        [FromBody] PayRequest request,
        CancellationToken token)
    {
        var command = request.ToCommand();
        return await Mediator.Send(command, token);
    }
    
    [HttpPost]
    [Route("capture")]
    public async Task<object> Capture(
        [FromBody] CaptureRequest request,
        CancellationToken token)
    {
        var command = request.ToCommand();
        return await Mediator.Send(command, token);
    }
}