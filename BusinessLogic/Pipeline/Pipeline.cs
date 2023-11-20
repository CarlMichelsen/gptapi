namespace BusinessLogic.Pipeline;

public abstract class Pipeline<T>
{
    private readonly List<Func<T, T>> stages = new();

    public Pipeline<T> AddStage(Func<T, T> stage)
    {
        this.stages.Add(stage);
        return this;
    }

    public T Execute(T input)
    {
        return this.stages
            .Aggregate(input, (current, stage) => stage(current));
    }
}
