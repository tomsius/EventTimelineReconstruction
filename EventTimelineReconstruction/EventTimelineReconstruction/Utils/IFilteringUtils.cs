using System.Collections.Generic;
using System.Windows.Controls;

namespace EventTimelineReconstruction.Utils;

public interface IFilteringUtils
{
    public bool AreAllChildrenChecked(List<CheckBox> children);
    public bool AreAllChildrenUnchecked(List<CheckBox> children);
    public List<CheckBox> GetChildrenCheckBoxes(CheckBox rootCheckBox);
    public CheckBox GetRootCheckBox(CheckBox item);
}