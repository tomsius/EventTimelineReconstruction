using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace EventTimelineReconstruction.Utils;

public class CustomDatePicker : DatePicker
{
    public string PlaceholderText
    {
        get
        {
            return (string)this.GetValue(WatermarkTextProperty);
        }
        set
        {
            this.SetValue(WatermarkTextProperty, value);
        }
    }

    public readonly static DependencyProperty WatermarkTextProperty = DependencyProperty.Register("PlaceholderText", typeof(string), typeof(CustomDatePicker), new PropertyMetadata("Select a Date"));

    public override void OnApplyTemplate()
    {
        base.OnApplyTemplate();
        DatePickerTextBox box = base.GetTemplateChild("PART_TextBox") as DatePickerTextBox;
        box.ApplyTemplate();

        ContentControl watermark = box.Template.FindName("PART_Watermark", box) as ContentControl;
        watermark.Content = PlaceholderText;
    }
}
