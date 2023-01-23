using EventTimelineReconstruction.Utils;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Windows;
using EventTimelineReconstruction.ViewModels;
using Xceed.Wpf.Toolkit;
using System.Windows.Media;

namespace EventTimelineReconstruction.Commands;

public sealed class InitializeColoursByTypeCommand : CommandBase
{
    private readonly ColourViewModel _colourViewModel;
    private readonly IColouringUtils _colouringUtils;

    public InitializeColoursByTypeCommand(ColourViewModel colourViewModel, IColouringUtils colouringUtils)
    {
        _colourViewModel = colourViewModel;
        _colouringUtils = colouringUtils;
    }

    public override void Execute(object parameter)
    {
        RoutedEventArgs e = parameter as RoutedEventArgs;
        Dictionary<TextBlock, ColorPicker> children = _colouringUtils.GetChildrenElements(e.OriginalSource as StackPanel);

        foreach (KeyValuePair<TextBlock, ColorPicker> pair in children)
        {
            string key = pair.Key.Text;
            Brush value = new SolidColorBrush(pair.Value.SelectedColor ?? Colors.Black);

            _colourViewModel.UpdateColourByType(key, value);
        }
    }
}
