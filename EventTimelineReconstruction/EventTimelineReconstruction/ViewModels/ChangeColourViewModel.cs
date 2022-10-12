using System.Windows.Input;
using System.Windows.Media;
using EventTimelineReconstruction.Commands;

namespace EventTimelineReconstruction.ViewModels;

public class ChangeColourViewModel : ViewModelBase
{
    private Color _selectedColour;

    public Color SelectedColour
    {
        get
        {
            return _selectedColour;
        }
        set
        {
            _selectedColour = value;
            this.OnPropertyChanged(nameof(SelectedColour));
        }
    }

    public ICommand ApplyCommand { get; }

    public ChangeColourViewModel(EventDetailsViewModel eventDetailsViewModel)
    {
        ApplyCommand = new ChangeEventColourCommand(this, eventDetailsViewModel);
    }

    public void SetBrushColour(Brush colour)
    {
        SolidColorBrush newColour = (SolidColorBrush)colour;
        SelectedColour = newColour.Color;
    }
}