using Orleans.Streams;
using Orleans.Streams.Core;

namespace EventSourcingExercise.Infrastructures.Projectors;

[ImplicitStreamSubscription(ProjectorName.TransactionRecord)]
public class TransactionRecordProjector : Grain, IGrainWithStringKey, IStreamSubscriptionObserver, IAsyncObserver<EventData>
{
    private readonly ILogger<TransactionRecordProjector> _logger;

    public TransactionRecordProjector(ILogger<TransactionRecordProjector> logger)
    {
        _logger = logger;
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<EventData>();
        await handle.ResumeAsync(this);
    }

    public Task OnNextAsync(EventData item, StreamSequenceToken? token = null)
    {
        _logger.LogInformation("GrainId: {GrainId}, IdentityString: {IdentityString}, RuntimeIdentity: {RuntimeIdentity}, Item: {@Item}",
            this.GetGrainId().Key.Value,
            IdentityString,
            RuntimeIdentity,
            item);
        return Task.CompletedTask;
    }

    public Task OnCompletedAsync()
    {
        _logger.LogInformation("Projector Completed!");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        _logger.LogError(ex, "Projector Error!");
        return Task.CompletedTask;
    }
}