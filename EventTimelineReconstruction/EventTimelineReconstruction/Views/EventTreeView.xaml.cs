using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using EventTimelineReconstruction.ViewModels;

namespace EventTimelineReconstruction.Views
{
    /// <summary>
    /// Interaction logic for EventTreeView.xaml
    /// </summary>
    public partial class EventTreeView : UserControl
    {
        EventViewModel _draggedItem;
        EventViewModel _target;
        TreeViewItem _draggedItemElement;

        public EventTreeView()
        {
            InitializeComponent();
        }

        private void TreeView_DragOver(object sender, DragEventArgs e)
        {
            TreeViewItem item = GetNearestContainer(e.OriginalSource as UIElement);

            if (item != null && CheckDropTarget(_draggedItem, item.Header as EventViewModel)) {
                e.Effects = DragDropEffects.Move;
            }
            else {
                e.Effects = DragDropEffects.None;
            }

            e.Handled = true;
        }

        private TreeViewItem GetNearestContainer(UIElement element)
        {
            // Walk up the element tree to the nearest tree view item.
            TreeViewItem container = element as TreeViewItem;

            while (container == null && element != null) {
                element = VisualTreeHelper.GetParent(element) as UIElement;
                container = element as TreeViewItem;
            }

            return container;
        }

        private bool CheckDropTarget(EventViewModel _sourceItem, EventViewModel _targetItem)
        {
            //Check whether the target item is meeting your condition
            // TODO - check all subtree above to disallow grouping from the same subtree
            return _sourceItem != _targetItem;
        }

        private void TreeView_Drop(object sender, DragEventArgs e)
        {
            e.Effects = DragDropEffects.None;
            e.Handled = true;

            // Verify that this is a valid drop and then store the drop target
            TreeViewItem targetItem = GetNearestContainer(e.OriginalSource as UIElement);

            if (targetItem != null && _draggedItem != null) {
                _target = targetItem.Header as EventViewModel;
                e.Effects = DragDropEffects.Move;
            }
        }

        private void TreeView_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton == MouseButtonState.Pressed) {
                _draggedItem = (EventViewModel)EventsTree.SelectedItem;
                _draggedItemElement = GetNearestContainer(e.OriginalSource as UIElement);

                if (_draggedItem != null) {
                    DragDropEffects finalDropEffect = DragDrop.DoDragDrop(EventsTree, EventsTree.SelectedValue, DragDropEffects.Move);

                    //Checking target is not null and item is dragging(moving)
                    if (finalDropEffect == DragDropEffects.Move && _target != null) {
                        // A Move drop was accepted
                        if (_draggedItem != _target) {
                            CopyItem(_draggedItem, _target);
                            _target = null;
                            _draggedItem = null;
                        }
                    }
                }
            }
        }

        private void CopyItem(EventViewModel _sourceItem, EventViewModel _targetItem)
        {
            //Asking user wether he want to drop the dragged TreeViewItem here or not
            if (MessageBox.Show("Would you like to drop " + _sourceItem.DisplayName + " into " + _targetItem.DisplayName + "?", "Confirmation", MessageBoxButton.YesNo) == MessageBoxResult.Yes) {
                //adding dragged TreeViewItem in target TreeViewItem
                AddChild(_sourceItem, _targetItem);
            }
        }

        public void AddChild(EventViewModel _sourceItem, EventViewModel _targetItem)
        {
            var parentItem = FindParent();

            if (parentItem == null) 
            {
                EventTreeViewModel vm = (EventTreeViewModel)DataContext;
                vm.Remove(_sourceItem);
            }
            else 
            {
                var parentViewModel = parentItem.Header as EventViewModel;
                parentViewModel.RemoveChild(_sourceItem);
            }

            _targetItem.AddChild(_sourceItem);
        }

        private TreeViewItem FindParent()
        {
            DependencyObject parent = VisualTreeHelper.GetParent(_draggedItemElement);
            while (!(parent is TreeViewItem || parent is TreeView)) {
                parent = VisualTreeHelper.GetParent(parent);
            }

            return (parent as ItemsControl) as TreeViewItem;
        }
    }
}
