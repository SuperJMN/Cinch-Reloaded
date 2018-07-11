using System;
using System.ComponentModel;
using System.Linq;
using Cinch.Reloaded.Commands;
using Cinch.Reloaded.Messaging;
using Cinch.Reloaded.Utilities;

namespace Cinch.Reloaded.Workspaces
{
    /// <summary>
    /// Workspace data class which can be used within a DataTemplate along
    /// with the NavProps DP to manage workspaces
    /// </summary>
    public partial class WorkspaceData : INotifyPropertyChanged, IDisposable
    {
        #region Data
        private string imagePath;
        private string viewLookupKey;
        private object dataValue;
        private string displayText;
        private SimpleCommand<Object, Object> closeWorkSpaceCommand;
        private Boolean isCloseable = true;        

        #endregion

        #region Ctor
        public WorkspaceData(string imagePath,string viewLookupKey, object dataValue, string displayText, bool isCloseable)
        {

            Mediator.Instance.Register(this);
            this.ImagePath = imagePath;
            this.ViewLookupKey = viewLookupKey;
            this.DataValue = dataValue;
            this.DisplayText = displayText;
            this.IsCloseable = isCloseable;

            CloseWorkSpaceCommand = new SimpleCommand<object, object>(x => true, x => ExecuteCloseWorkSpaceCommand(x));
        }
        #endregion

        #region Custom Closing Event

        public event CancelEventHandler WorkspaceTabClosing;

        protected void NotifyWorkspaceTabClosing(CancelEventArgs args)
        {
            CancelEventHandler handler = WorkspaceTabClosing;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        #endregion

        #region Command Implememtation
        /// <summary>
        /// Executes the CloseWorkSpace Command
        /// </summary>
        private void ExecuteCloseWorkSpaceCommand(object o)
        {
            CancelEventHandler handler = WorkspaceTabClosing;
            if (handler != null && WorkspaceTabClosing.GetInvocationList().Count() > 0)
            {
                CancelEventArgs args = new CancelEventArgs(false);
                NotifyWorkspaceTabClosing(args);
                if (args.Cancel == false)
                {
                    Mediator.Instance.NotifyColleagues<WorkspaceData>("RemoveWorkspaceItem", this);
                }
            }
            else
            {
                Mediator.Instance.NotifyColleagues<WorkspaceData>("RemoveWorkspaceItem", this);
            }
        }
        #endregion

        #region Public Properties

        /// <summary>
        /// The ViewModel that was created for this WorkSpaceData object, if it was used to create a View
        /// </summary>

        #region Property ViewModelInstance
        private object viewModelInstance;
        public static readonly PropertyChangedEventArgs ViewModelInstanceEventArgs = ObservableHelper.CreateArgs<WorkspaceData>(x => x.ViewModelInstance);
        public object ViewModelInstance
        {
            get { return viewModelInstance; }
            set
            {

                if (object.ReferenceEquals(viewModelInstance, value)) return;
                viewModelInstance = value;
                NotifyPropertyChanged(ViewModelInstanceEventArgs);
            }
        }
        #endregion



        /// <summary>
        /// CloseActivePopUpCommand : Close popup command
        /// </summary>
        public SimpleCommand<Object, Object> CloseWorkSpaceCommand { get; private set; }

        /// <summary>
        /// Is this workspace a closeable workspace
        /// </summary>
        static PropertyChangedEventArgs isCloseableArgs =
            ObservableHelper.CreateArgs<WorkspaceData>(x => x.IsCloseable);

        public Boolean IsCloseable
        {
            get { return isCloseable; }
            set
            {
                isCloseable = value;
                NotifyPropertyChanged(isCloseableArgs);
            }
        }

        /// <summary>
        /// True if this workspace has an image
        /// </summary>
        public bool HasImage
        {
            get
            {
                return !string.IsNullOrEmpty(ImagePath);
            }
        }


        /// <summary>
        /// ImagePath
        /// </summary>
        static PropertyChangedEventArgs imagePathArgs =
            ObservableHelper.CreateArgs<WorkspaceData>(x => x.ImagePath);

        public string ImagePath
        {
            get { return imagePath; }
            set
            {
                imagePath = value;
                NotifyPropertyChanged(imagePathArgs);
            }
        }



        /// <summary>
        /// View key lookup name
        /// </summary>
        static PropertyChangedEventArgs viewLookupKeyArgs =
            ObservableHelper.CreateArgs<WorkspaceData>(x => x.ViewLookupKey);

        public string ViewLookupKey
        {
            get { return viewLookupKey; }
            set
            {
                viewLookupKey = value;
                NotifyPropertyChanged(viewLookupKeyArgs);
            }
        }


        /// <summary>
        /// Workspace context data
        /// </summary>
        static PropertyChangedEventArgs dataValueArgs =
            ObservableHelper.CreateArgs<WorkspaceData>(x => x.DataValue);

        public object DataValue
        {
            get { return dataValue; }
            set
            {
                dataValue = value;
                NotifyPropertyChanged(dataValueArgs);
            }
        }


        /// <summary>
        /// Workspace display text, is used for Headers controls such as TabControl
        /// </summary>
        static PropertyChangedEventArgs displayTextArgs =
            ObservableHelper.CreateArgs<WorkspaceData>(x => x.DisplayText);

        public string DisplayText
        {
            get { return displayText; }
            set
            {
                displayText = value;
                NotifyPropertyChanged(displayTextArgs);
            }
        }
        #endregion

        #region Overrides
        public override string ToString()
        {
            return String.Format("ViewLookupKey {0}, DisplayText {1}, IsCloseable {2}",
                ViewLookupKey, DisplayText, IsCloseable);
        }
        #endregion

        #region INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Notify using pre-made PropertyChangedEventArgs
        /// </summary>
        /// <param name="args"></param>
        protected void NotifyPropertyChanged(PropertyChangedEventArgs args)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, args);
            }
        }

        /// <summary>
        /// Notify using String property name
        /// </summary>
        protected void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;

            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Invoked when this object is being removed from the application
        /// and will be subject to garbage collection.
        /// </summary>
        public void Dispose()
        {
            Mediator.Instance.Unregister(this);
            this.OnDispose();
        }

        /// <summary>
        /// Child classes can override this method to perform 
        /// clean-up logic, such as removing event handlers.
        /// </summary>
        protected virtual void OnDispose()
        {
        }

#if DEBUG
        /// <summary>
        /// Useful for ensuring that ViewModel objects are properly garbage collected.
        /// </summary>
        ~WorkspaceData()
        {

        }
#endif

        #endregion // IDisposable Members
    }
}
