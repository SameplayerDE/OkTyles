using System.Collections.Generic;
using OkTyles.Core.Commands;

namespace OkTyles.Core.Commands;

public class CommandInvoker
{
    private readonly Stack<Command> _undoStack;
    private readonly Stack<Command> _redoStack;

    public CommandInvoker()
    {
        _undoStack = new Stack<Command>();
        _redoStack = new Stack<Command>();
    }

    public void ExecuteCommand(Command command)
    {
        _undoStack.Push(command);
        _redoStack.Clear();
        command.Execute();
    }

    public void Undo()
    {
        if (_undoStack.Count > 0)
        {
            Command command = _undoStack.Pop();
            _redoStack.Push(command);
            command.Undo();
        }
    }

    public void Redo()
    {
        if (_redoStack.Count > 0)
        {
            Command command = _redoStack.Pop();
            _undoStack.Push(command);
            command.Execute();
        }
    }
}