using System.Text.Json.Serialization.Metadata;

namespace EventSourcingExercise.Infrastructures;

public class TypeMapper
{
    private readonly Dictionary<string, Type> _aggregateRootNameToTypeMapping = new();
    private readonly Dictionary<Type, string> _aggregateRootTypeToNameMapping = new();
    private readonly Dictionary<string, Type> _eventNameToTypeMapping = new();
    private readonly Dictionary<Type, string> _eventTypeToNameMapping = new();

    public void AddAggregateRoot(string aggregateRootName, Type type)
    {
        _aggregateRootNameToTypeMapping.Add(aggregateRootName, type);
        _aggregateRootTypeToNameMapping.Add(type, aggregateRootName);
    }

    public Type GetAggregateRootType(string aggregateRootName)
    {
        return _aggregateRootNameToTypeMapping[aggregateRootName];
    }

    public string GetAggregateRootName(Type type)
    {
        return _aggregateRootTypeToNameMapping[type];
    }

    public void AddEvent(string eventName, Type type)
    {
        _eventNameToTypeMapping.Add(eventName, type);
        _eventTypeToNameMapping.Add(type, eventName);
    }

    public Type GetEventType(string eventName)
    {
        return _eventNameToTypeMapping[eventName];
    }

    public string GetEventName(Type type)
    {
        return _eventTypeToNameMapping[type];
    }
}