using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interactivity;
using System.Windows.Threading;
using Cinch.Reloaded.ViewModels;

namespace Cinch.Reloaded.Interactivity.Behaviors
{
    public class DataGridRowFocusBehavior : Behavior<DataGrid>
    {

        private INotifyCollectionChanged collection;

        protected override void OnAttached()
        {
            collection = (INotifyCollectionChanged)AssociatedObject.ItemsSource;
            collection.CollectionChanged += CollectionOnCollectionChanged;

        }

        private void CollectionOnCollectionChanged(object sender, NotifyCollectionChangedEventArgs notifyCollectionChangedEventArgs)
        {
            if (notifyCollectionChangedEventArgs.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var viewModel in notifyCollectionChangedEventArgs.NewItems.Cast<ViewModelBase>())
                {
                    viewModel.FocusRequested += ViewModelOnFocusRequested;
                }
            }
        }

        private void ViewModelOnFocusRequested(object sender, string s)
        {
            AssociatedObject.ScrollIntoView(sender);

            var row = (DataGridRow)AssociatedObject.ItemContainerGenerator.ContainerFromItem(sender);


            // Delay the call to allow the current batch
            // of processing to finish before we shift focus.
            AssociatedObject.Dispatcher.BeginInvoke((Action)delegate
            {

                if (!AssociatedObject.Focus())
                {
                    DependencyObject fs = FocusManager.GetFocusScope(AssociatedObject);
                    FocusManager.SetFocusedElement(fs, AssociatedObject);
                    AssociatedObject.CurrentCell = new DataGridCellInfo(sender,
                                      AssociatedObject.Columns[0]);
                    AssociatedObject.BeginEdit();
                }
            },
            DispatcherPriority.Background);

            AssociatedObject.CurrentCell = new DataGridCellInfo(sender,
                                      AssociatedObject.Columns[0]);
            AssociatedObject.BeginEdit();
        }

        private void ItemContainerGeneratorOnStatusChanged(object sender, EventArgs eventArgs)
        {


        }

        private void ItemContainerGeneratorOnItemsChanged(object sender, ItemsChangedEventArgs itemsChangedEventArgs)
        {

        }
    }
}