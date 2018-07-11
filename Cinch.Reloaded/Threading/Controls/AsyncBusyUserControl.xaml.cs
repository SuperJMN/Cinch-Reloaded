using System.Windows;
using System.Windows.Controls;

namespace Cinch.Reloaded.Threading.Controls
{
    /// <summary>
    /// Control that is shown while threading 
    /// operation is in progress
    /// </summary>
    public partial class AsyncBusyUserControl : UserControl
    {

        #region Constructor
        public AsyncBusyUserControl()
        {
            InitializeComponent();
        }
        #endregion

        #region AsyncWaitText

        /// <summary>
        /// AsyncWaitText Dependency Property
        /// </summary>
        public static readonly DependencyProperty AsyncWaitTextProperty =
            DependencyProperty.Register("AsyncWaitText", typeof(string), typeof(AsyncBusyUserControl),
                new FrameworkPropertyMetadata((string)"",
                    new PropertyChangedCallback(OnAsyncWaitTextChanged)));

        /// <summary>
        /// Gets or sets the AsyncWaitText property.  
        /// </summary>
        public string AsyncWaitText
        {
            get { return (string)GetValue(AsyncWaitTextProperty); }
            set { SetValue(AsyncWaitTextProperty, value); }
        }

        /// <summary>
        /// Handles changes to the AsyncWaitText property.
        /// </summary>
        private static void OnAsyncWaitTextChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AsyncBusyUserControl)d).lblWait.Content = (string)e.NewValue;
        }



        #endregion




    }

}
