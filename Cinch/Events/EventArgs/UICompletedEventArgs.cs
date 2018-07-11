/// <summary>
/// This is the EventArgs return value for the IUIVisualizer.Show completed event.
/// </summary>
namespace CinchExtended.Events.EventArgs
{
    /// <summary>
    /// This is the EventArgs return value for the IUIVisualizer.Show completed event.
    /// </summary>
    public class UICompletedEventArgs : System.EventArgs
    {
        #region Public Properties
        /// <summary>
        /// Data passed to the Show method.
        /// </summary>
        public object State { get; set; }
        /// <summary>
        /// Final result of the UI dialog
        /// </summary>
        public bool? Result { get; set; }
        #endregion
    }
}
