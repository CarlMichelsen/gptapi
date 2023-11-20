namespace Interface.Pipeline;

public interface IPipeline<T>
{
    Task<T> Execute(T input);
}
