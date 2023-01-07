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
    private readonly IColouringUtils _colouringUtils;

    public SelectedColourChangedCommand(ColourViewModel colourViewModel, IColouringUtils colouringUtils)
    {
        _colourViewModel = colourViewModel;
        _colouringUtils = colouringUtils;
    }

    public override void Execute(object parameter)
    {
        RoutedEventArgs e = parameter as RoutedEventArgs;
        ColorPicker colorPicker = e.OriginalSource as ColorPicker;
        TextBlock textBlock = _colouringUtils.GetSiblingTextBlock(colorPicker);

        string key = (string)textBlock.Tag;
        Brush value = new SolidColorBrush(colorPicker.SelectedColor ?? Colors.Black);
        _colourViewModel.UpdateColourByType(key, value);
    }
}
