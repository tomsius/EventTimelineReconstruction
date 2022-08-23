using System.Threading.Tasks;

namespace EventTimelineReconstruction.Commands;

public abstract class AsyncCommandBase : CommandBase
{
    private bool _isExecuting;

    private bool IsExecuting
    {
        get
        {
            return _isExecuting;
        }
        set
        {
            _isExecuting = value;
            this.OnCanExecuteChanged();
        }
    }

    public override bool CanExecute(object parameter)
    {
        return !IsExecuting && base.CanExecute(parameter);
    }

    public override async void Execute(object parameter)
    {
        IsExecuting = true;

        try 
        {
            await this.ExecuteAsync(parameter);
        }
        finally
        {
            IsExecuting = false;
        }
    }

    public abstract Task ExecuteAsync(object parameter);
}
