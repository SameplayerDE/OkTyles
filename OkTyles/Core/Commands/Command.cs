namespace OkTyles.Core.Commands;

public abstract class Command
{
    public abstract bool Execute();
    public abstract void Undo();
}