using System.Text.Json;
using EventSourcingExercise.Infrastructures.EventSourcing.BackgroundServices.EventDeliveries;
using EventSourcingExercise.Utilities;
using Microsoft.EntityFrameworkCore;
using Orleans.Streams;
using Orleans.Streams.Core;

namespace EventSourcingExercise.Infrastructures.Projectors;

public class ProjectorBase<TEntry> :
    Grain,
    IGrainWithStringKey,
    IStreamSubscriptionObserver,
    IAsyncObserver<EventItem>
    where TEntry : ProjectorEntryBase
{
    protected readonly ILogger Logger;
    private readonly TypeMapper _typeMapper;
    private readonly IDbContextFactory<ProjectorDbContext> _dbContextFactory;
    private readonly Dictionary<string, Delegate> _eventFunctions = new();
    private ProjectorDbContext? _projectorDbContext;

    protected ProjectorBase(ILogger logger, TypeMapper typeMapper, IDbContextFactory<ProjectorDbContext> dbContextFactory)
    {
        Logger = logger;
        _typeMapper = typeMapper;
        _dbContextFactory = dbContextFactory;
    }

    public async Task OnSubscribed(IStreamSubscriptionHandleFactory handleFactory)
    {
        var handle = handleFactory.Create<EventItem>();
        await handle.ResumeAsync(this);
    }

    public async Task OnNextAsync(EventItem item, StreamSequenceToken? token = null)
    {
        Logger.LogInformation("GrainId: {GrainId}, IdentityString: {IdentityString}, RuntimeIdentity: {RuntimeIdentity}, Item: {@Item}",
            this.GetGrainId().Key.Value,
            IdentityString,
            RuntimeIdentity,
            item);

        var eventType = _typeMapper.GetEventType(item.EventName);
        dynamic evt = JsonSerializer.Deserialize(item.EventText, eventType, JsonOptions.WebOptions)!;

        _projectorDbContext ??= await _dbContextFactory.CreateDbContextAsync();

        try
        {
            await InvokeFunc(item, evt);
            await _projectorDbContext.SaveChangesAsync();
        }
        catch
        {
            await _projectorDbContext.DisposeAsync();
            _projectorDbContext = null;
            throw;
        }
    }

    public Task OnCompletedAsync()
    {
        Logger.LogInformation("Projector Completed!");
        return Task.CompletedTask;
    }

    public Task OnErrorAsync(Exception ex)
    {
        Logger.LogError(ex, "Projector Error!");
        return Task.CompletedTask;
    }

    protected void Create<TEvent>(Func<EventData<TEvent>, Task<TEntry>> func)
    {
        var eventName = _typeMapper.GetEventName(typeof(TEvent));
        _eventFunctions.Add(eventName, func);
    }

    protected void Create<TEvent>(Func<EventData<TEvent>, TEntry> func)
    {
        var eventName = _typeMapper.GetEventName(typeof(TEvent));
        _eventFunctions.Add(eventName, func);
    }

    protected void Apply<TEvent>(Action<EventData<TEvent>, TEntry> func)
    {
        var eventName = _typeMapper.GetEventName(typeof(TEvent));
        _eventFunctions.Add(eventName, func);
    }

    protected void Apply<TEvent>(Func<EventData<TEvent>, TEntry, Task> func)
    {
        var eventName = _typeMapper.GetEventName(typeof(TEvent));
        _eventFunctions.Add(eventName, func);
    }

    private async Task InvokeFunc<TEvent>(EventItem item, TEvent evt)
    {
        var eventData = GetEventData(item, evt);

        if (!_eventFunctions.TryGetValue(item.EventName, out var funcDelegate))
        {
            Logger.LogWarning("無對應的委派方法. eventName :{EventName}", item.EventName);
            return;
        }

        if (funcDelegate is Func<EventData<TEvent>, Task<TEntry>> createFuncAsync)
        {
            var entry = await createFuncAsync(eventData);
            await AddNewEntry(entry, eventData);
        }
        else if (funcDelegate is Func<EventData<TEvent>, TEntry> creatFunc)
        {
            var entry = creatFunc(eventData);
            await AddNewEntry(entry, eventData);
        }
        else if (funcDelegate is Action<EventData<TEvent>, TEntry> applyFunc)
        {
            var entry = await GetEntry(eventData);
            applyFunc(eventData, entry);
            entry.Version = eventData.Version;
        }
        else if (funcDelegate is Func<EventData<TEvent>, TEntry, Task> applyFuncAsync)
        {
            var entry = await GetEntry(eventData);
            await applyFuncAsync(eventData, entry);
            entry.Version = eventData.Version;
        }
    }

    private async Task<TEntry> GetEntry<TEvent>(EventData<TEvent> eventData)
    {
        var entry = await _projectorDbContext!.Set<TEntry>().FindAsync(eventData.StreamId);
        if (entry == null)
        {
            // todo: 需處理找不到的狀況
            throw new ArgumentException();
        }

        return entry;
    }

    private async Task AddNewEntry<TEvent>(TEntry entry, EventData<TEvent> eventData)
    {
        entry.TenantCode = eventData.TenantCode;
        entry.StreamId = eventData.StreamId;
        entry.Version = eventData.Version;
        entry.CreatedAt = eventData.CreatedAt;

        await _projectorDbContext!.AddAsync(entry);
    }

    private static EventData<TEvent> GetEventData<TEvent>(EventItem item, TEvent evt)
    {
        return new EventData<TEvent>
        {
            Event = evt,
            StreamId = item.StreamId,
            EventId = item.EventId,
            TenantCode = item.TenantCode,
            Version = item.Version,
            CreatedAt = item.CreatedAt,
        };
    }
}