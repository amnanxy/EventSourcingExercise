using EventSourcingExercise.Utilities.IdGenerators;
using NanoidDotNet;

namespace EventSourcingExercise.Infrastructures.IdGenerators;

public class TextIdGenerator : ITextIdGenerator
{
    public string CreateId(string prefix, int randomLength)
    {
        var utcNow = DateTime.UtcNow;

        return $"{prefix}{utcNow.ToString("yyyy")[1..]}{GetMonthText(utcNow)}{utcNow:dd}{Nanoid.Generate(Nanoid.Alphabets.UppercaseLettersAndDigits, randomLength)}";
    }

    private static string GetMonthText(DateTime utcNow)
    {
        return utcNow.Month switch
        {
            10 => "A",
            11 => "J",
            12 => "Q",
            _ => utcNow.Month.ToString(),
        };
    }
}