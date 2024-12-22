using Orleans.Streams;
using Orleans.Streams.Core;

namespace EventSourcingExercise.Infrastructures.Projectors;

[ImplicitStreamSubscription(ProjectorName.TransactionRecord)]
public class TransactionRecordProjector : Grain, IGrainWithStringKey, IStreamSubscriptionObserver, IAsyncObserver<EventEntry>
{
    private readonly ILogger<TransactionRecordProjector> _logger;

    public TransactionRecordProjector(ILogger<TransactionRecordProjector> logger)
    {
        _logger = logger;
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<EventEntry>();
        await handle.ResumeAsync(this);
    }

    public Task OnNextAsync(EventEntry entry, StreamSequenceToken? token = null)
    {
        _logger.LogInformation("GrainId: {GrainId}, IdentityString: {IdentityString}, RuntimeIdentity: {RuntimeIdentity}, Item: {@Item}",
            this.GetGrainId().Key.Value,
            IdentityString,
            RuntimeIdentity,
            entry);
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