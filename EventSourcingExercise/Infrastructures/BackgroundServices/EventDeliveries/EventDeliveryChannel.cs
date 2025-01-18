using System.Threading.Channels;

namespace EventSourcingExercise.Infrastructures.BackgroundServices.EventDeliveries;

public class EventDeliveryChannel
{
    private readonly Channel<EventDeliveryPackage> _channel = Channel.CreateBounded<EventDeliveryPackage>(new BoundedChannelOptions(10000)
    {
        FullMode = BoundedChannelFullMode.Wait,
    });

    public IAsyncEnumerable<EventDeliveryPackage> Read(CancellationToken cancellationToken)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }

    public async Task Write(EventDeliveryPackage package)
    {
        await _channel.Writer.WriteAsync(package);
    }

    public void Close()
    {
        _channel.Writer.Complete();
    }
}