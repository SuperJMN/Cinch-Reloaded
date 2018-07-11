using System.Windows;
using System.Windows.Interactivity;
using CinchExtended.Commands;

namespace CinchExtended.Interactivity.Triggers
{
    /// <summary>
    /// Is a Trigger that invokes its action based on listening to a bound ViewModels
    /// SimpleCommand.CommandCompleted event
    /// </summary>
    /// <remarks>
    /// Recommended usage:
    /// <code>
    /// IN YOUR VIEW HAVE SOMETHING LIKE THIS
    /// 
    /// 
    ///         xmlns:i="clr-namespace:System.Windows.Interactivity;assembly=System.Windows.Interactivity"
    ///         xmlns:CinchV2="clr-namespace:Cinch;assembly=Cinch.WPF"
    ///         
    ///
    ///    <TabControl x:Name="tabControl">
    ///        <i:Interaction.Triggers>
    ///	        <CinchV2:CompletedAwareCommandTrigger Command="{Binding SomeVMCommand}">
    ///		        <!-- Some Action of your choice indexer here -->
    ///	        </CinchV2:CompletedAwareGoToStateCommandTrigger>
    ///        </i:Interaction.Triggers>
    ///    </TabControl>
    ///         
    /// </code>
    /// </remarks>
    public class CompletedAwareCommandTrigger : TriggerBase<FrameworkElement>
    {
        #region DPs

        #region Command
        public static readonly DependencyProperty CommandProperty =
            DependencyProperty.Register("Command", typeof(ICompletionAwareCommand),
                typeof(CompletedAwareCommandTrigger), null);

        /// <summary>
        /// Gets or sets the Command property. 
        /// </summary>
        public ICompletionAwareCommand Command
        {
            get { return (ICompletionAwareCommand)GetValue(CommandProperty); }
            set { SetValue(CommandProperty, value); }
        }
        #endregion

        #endregion

        #region Overrides
        /// <summary>
        /// Called after the behavior is attached to an AssociatedObject.
        /// </summary>
        /// <remarks>
        /// Override this to hook up functionality to the AssociatedObject.
        /// </remarks>
        protected override void OnAttached()
        {
            base.OnAttached();
            this.Command.CommandCompleted += Command_Completed;

        }


        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.Command.CommandCompleted -= Command_Completed;
        }
        #endregion

        #region Private Method
        private void Command_Completed(object parameter)
        {

            // Invoke the actions
            InvokeActions(parameter);
        }
        #endregion
    }
}
