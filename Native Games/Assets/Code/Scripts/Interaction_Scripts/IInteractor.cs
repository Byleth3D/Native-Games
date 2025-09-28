
public interface IInteractor
{
    InteractorType InteractorType { get; }
}

public enum InteractorType
{
    System,
    Agent
}