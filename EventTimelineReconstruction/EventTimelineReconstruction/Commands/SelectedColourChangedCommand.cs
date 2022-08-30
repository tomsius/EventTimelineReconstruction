using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using EventTimelineReconstruction.Utils;
using EventTimelineReconstruction.ViewModels;
using Xceed.Wpf.Toolkit;

namespace EventTimelineReconstruction.Commands;

public class SelectedColourChangedCommand : CommandBase
{
    private readonly ColourViewModel _colourViewModel;

    public SelectedColourChangedCommand(ColourViewModel colourViewModel)
    {
        _colourViewModel = colourViewModel;
    }

    public override void Execute(object parameter)
    {
        RoutedEventArgs e = parameter as RoutedEventArgs;
        ColorPicker colorPicker = e.OriginalSource as ColorPicker;
        TextBlock textBlock = ColouringUtils.GetSiblingTextBlock(colorPicker);

        string key = textBlock.Text;
        Brush value = new SolidColorBrush(colorPicker.SelectedColor ?? Colors.Black);
        _colourViewModel.UpdateColourByType(key, value);
    }
}
