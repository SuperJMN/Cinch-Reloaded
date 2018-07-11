using System.Windows;
using System.Windows.Controls;

namespace Cinch.Reloaded.Threading.Controls
{
    /// <summary>
    /// Control that is shown when the threading 
    /// operation fails
    /// </summary>
    public partial class AsyncFailedUserControl : UserControl
    {
        #region Constructor
        public AsyncFailedUserControl()
        {

            InitializeComponent();
        }
        #endregion


        #region Error

        /// <summary>
        /// Error Dependency Property
        /// </summary>
        public static readonly DependencyProperty ErrorProperty =
            DependencyProperty.Register("Error", typeof(string), typeof(AsyncFailedUserControl),
                new FrameworkPropertyMetadata((string)"",
                    new PropertyChangedCallback(OnErrorChanged)));

        /// <summary>
        /// Gets or sets the Error property.  
        /// </summary>
        public string Error
        {
            get { return (string)GetValue(ErrorProperty); }
            set { SetValue(ErrorProperty, value); }
        }

        /// <summary>
        /// Handles changes to the Error property.
        /// </summary>
        private static void OnErrorChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AsyncFailedUserControl)d).txtError.Text = (string)e.NewValue;
        }
        #endregion




    }

}
