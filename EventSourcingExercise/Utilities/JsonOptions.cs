using System.Text.Json;

namespace EventSourcingExercise.Utilities;

public static class JsonOptions
{
    public static readonly JsonSerializerOptions WebOptions = new(JsonSerializerDefaults.Web);
}