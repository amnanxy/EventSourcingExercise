using EventSourcingExercise.Transactions.ApiModels.Pays;
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
}