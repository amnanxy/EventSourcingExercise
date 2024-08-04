using EventSourcingExercise.Utilities.IdGenerators;
using IdGen;

namespace EventSourcingExercise.Infrastructures.IdGenerators;

public class NumberIdGenerator : INumberIdGenerator
{
    private readonly IdGenerator _idGenerator;

    public NumberIdGenerator(int generatorId)
    {
        _idGenerator = new IdGenerator(generatorId);
    }

    public long CreateId()
    {
        return _idGenerator.CreateId();
    }
}