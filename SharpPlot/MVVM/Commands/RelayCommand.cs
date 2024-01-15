using System;

namespace SharpPlot.MVVM.Commands;

public class RelayCommand : Command
{
    private readonly Action<object?> _execute;
    private readonly Predicate<object?>? _canExecute;
    
    private RelayCommand(Action<object?> execute, Predicate<object?>? canExecute)
    {
        _execute = execute;
        _canExecute = canExecute;
    }
    
    public static RelayCommand Create(Action<object?> execute, Predicate<object?>? canExecute = null)
        => new(execute, canExecute);
    
    public override bool CanExecute(object? parameter) 
        => _canExecute == null || parameter == null || _canExecute(parameter);

    public override void Execute(object? parameter)
    {
        if (!CanExecute(parameter))
        {
            return;
        }
        
        _execute(parameter);
    }
}