using System.Windows;
using System.Windows.Controls;
using System.Windows.Interactivity;

namespace PassManager.Views.Behavior
{
    public class TreeViewSelectedItemBindingBehavior : Behavior<TreeView>
    {
        public object SelectedItem
        {
            get { return GetValue(SelectedItemProperty); }
            set { SetValue(SelectedItemProperty, value); }
        }

        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register(
                nameof(SelectedItem),
                typeof(object),
                typeof(TreeViewSelectedItemBindingBehavior),
                new FrameworkPropertyMetadata(null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedItemChanged));

        private static void OnSelectedItemChanged(
            DependencyObject sender,
            DependencyPropertyChangedEventArgs e)
        {
            var behavior = (TreeViewSelectedItemBindingBehavior)sender;
            if (behavior.AssociatedObject == null) return;

            var generator = behavior.AssociatedObject.ItemContainerGenerator;

            if(generator.ContainerFromItem(e.NewValue) is TreeViewItem item)
            {
                item.SetValue(TreeViewItem.IsSelectedProperty, true);
            }
        }

        protected override void OnAttached()
        {
            base.OnAttached();
            AssociatedObject.SelectedItemChanged += OnTreeViewSelectedItemChanged;
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            if(AssociatedObject != null)
            {
                AssociatedObject.SelectedItemChanged -= OnTreeViewSelectedItemChanged;
            }
        }

        private void OnTreeViewSelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
            => SelectedItem = e.NewValue;
    }
}
