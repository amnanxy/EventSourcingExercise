using IdGen;

namespace EventSourcingExercise.Utilities.IdGenerators;

public class NumberIdGenerator : IIdGenerator<long>
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