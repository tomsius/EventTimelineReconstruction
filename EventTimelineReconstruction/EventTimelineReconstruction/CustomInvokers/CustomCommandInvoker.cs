using System.Windows;
using System.Windows.Input;
using Microsoft.Xaml.Behaviors;

namespace EventTimelineReconstruction.CustomInvokers;

public sealed class CustomCommandInvoker : TriggerAction<DependencyObject>
{
    public readonly static DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(CustomCommandInvoker), null);

    public ICommand Command
    {
        get
        {
            return (ICommand)this.GetValue(CommandProperty);
        }
        set
        {
            this.SetValue(CommandProperty, value);
        }
    }

    protected override void Invoke(object parameter)
    {
        if (AssociatedObject != null) {
            ICommand command = Command;

            if ((command != null) && command.CanExecute(parameter)) {
                command.Execute(parameter);
            }
        }
    }
}
