using EventTimelineReconstruction.Utils;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using EventTimelineReconstruction.ViewModels;
using Xceed.Wpf.Toolkit;
using System.Windows.Media;

namespace EventTimelineReconstruction.Commands;

public class InitializeColoursByTypeCommand : CommandBase
{
    private readonly ColourViewModel _colourViewModel;

    public InitializeColoursByTypeCommand(ColourViewModel colourViewModel)
    {
        _colourViewModel = colourViewModel;
    }

    public override void Execute(object parameter)
    {
        RoutedEventArgs e = parameter as RoutedEventArgs;
        Dictionary<TextBlock, ColorPicker> children = ColouringUtils.GetChildrenElements(e.OriginalSource as StackPanel);

        foreach (KeyValuePair<TextBlock, ColorPicker> pair in children)
        {
            string key = pair.Key.Text;
            Brush value = new SolidColorBrush(pair.Value.SelectedColor ?? Colors.Black);

            _colourViewModel.UpdateColourByType(key, value);
        }
    }
}
