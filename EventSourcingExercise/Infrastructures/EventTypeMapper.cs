namespace EventSourcingExercise.Infrastructures;

public class EventTypeMapper
{
    private readonly Dictionary<string, Type> _nameToTypeMapping = new();
    private readonly Dictionary<Type, string> _typeToNameMapping = new();

    public Type this[string eventName] => _nameToTypeMapping[eventName];

    public string this[Type type] => _typeToNameMapping[type];

    public void Add(string eventName, Type type)
    {
        _nameToTypeMapping.Add(eventName, type);
        _typeToNameMapping.Add(type, eventName);
    }
}