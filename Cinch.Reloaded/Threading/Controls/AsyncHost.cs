using System.Windows;
using System.Windows.Controls;

namespace Cinch.Reloaded.Threading.Controls
{
    public enum AsyncType { Content = 1, Busy, Error };


    /// <summary>
    /// A generic threading host container that supports content/busy/error controls
    /// An example of how to use this is shown here
    /// </summary>
    /// <example>
    /// 
    ///     xmlns:cinch="clr-namespace:Cinch;assembly=Cinch.WPF"
    ///    <cinch:AsyncHost AsyncState="{Binding Path=AsyncState, Mode=OneWay}">
    ///        <Grid controls:AsyncHost.AsyncContentType="Content"
    ///            Background="White">
    ///
    ///    </Grid>
    ///
    ///        <cinch:AsyncBusyUserControl 
    ///                controls:AsyncHost.AsyncContentType="Busy" 
    ///                AsyncWaitText="{Binding Path=WaitText, Mode=OneWay}" 
    ///                Visibility="Hidden" />
    ///        <cinch:AsyncFailedUserControl 
    ///                controls:AsyncHost.AsyncContentType="Error" 
    ///                Error="{Binding Path=ErrorText, Mode=OneWay}" 
    ///                Visibility="Hidden" />
    ///
    ///    </cinch>
    ///
    ///
    ///  Where you may have a ViewModel something like this
    ///
    ///
    ///
    ///using System;
    ///using System.Threading.Tasks;
    ///using System.ComponentModel;
    ///using Cinch;
    ///
    ///namespace SomeNameSpace
    ///{
    ///    public class SomeViewModel : Cinch.ViewModelBase
    ///    {
    ///        private string waitText;
    ///        private string errorMessage;
    ///        private AsyncType asyncState = AsyncType.Content;
    ///
    ///
    ///        //This method is called to do some work in the background
    ///        private void DoWork()
    ///        {
    ///            AsyncState = AsyncType.Busy;
    ///            WaitText = "Doing work";
    ///            try
    ///            {
    ///                Task t = Task.Factory.StartNew(() =>
    ///                    {
    ///                        //do work
    ///                    });
    ///                t.ContinueWith(ant =>
    ///                {
    ///                    AsyncState = AsyncType.Content;
    ///                }, TaskContinuationOptions.OnlyOnRanToCompletion);
    ///            }
    ///            catch (AggregateException aggex)
    ///            {
    ///                AsyncState = AsyncType.Error;
    ///            }
    ///        }
    ///
    ///
    ///
    ///        /// <summary>
    ///        /// AsyncState
    ///        /// </summary>
    ///        static PropertyChangedEventArgs asyncStateArgs =
    ///            ObservableHelper.CreateArgs<SomeViewModel>(x => x.AsyncState);
    ///
    ///        public AsyncType AsyncState
    ///        {
    ///            get { return asyncState; }
    ///            private set
    ///            {
    ///                asyncState = value;
    ///                NotifyPropertyChanged(asyncStateArgs);
    ///            }
    ///        }
    ///
    ///
    ///
    ///        /// <summary>
    ///        /// WaitText
    ///        /// </summary>
    ///        static PropertyChangedEventArgs waitTextArgs =
    ///            ObservableHelper.CreateArgs<SomeViewModel>(x => x.WaitText);
    ///
    ///   
    ///        public string WaitText
    ///        {
    ///            get { return waitText; }
    ///            private set
    ///            {
    ///                waitText = value;
    ///                NotifyPropertyChanged(waitTextArgs);
    ///            }
    ///        }
    ///
    ///
    ///        /// <summary>
    ///        /// ErrorMessage
    ///        /// </summary>
    ///        static PropertyChangedEventArgs errorMessageArgs =
    ///            ObservableHelper.CreateArgs<SomeViewModel>(x => x.ErrorMessage);
    ///
    ///
    ///        public string ErrorMessage
    ///        {
    ///            get { return errorMessage; }
    ///            private set
    ///            {
    ///                errorMessage = value;
    ///                NotifyPropertyChanged(errorMessageArgs);
    ///            }
    ///        }
    ///
    ///    }
    ///}
    ///
    /// </example>
    public class AsyncHost : Grid
    {
        #region ShouldCollapse

        /// <summary>
        /// ShouldCollapse Dependency Property
        /// </summary>
        public static readonly DependencyProperty ShouldCollapseProperty =
            DependencyProperty.Register("ShouldCollapse", typeof(bool), typeof(AsyncHost),
                new FrameworkPropertyMetadata((bool)false));

        /// <summary>
        /// Gets or sets the ShouldCollapse property. 
        /// </summary>
        public bool ShouldCollapse
        {
            get { return (bool)GetValue(ShouldCollapseProperty); }
            set { SetValue(ShouldCollapseProperty, value); }
        }

        #endregion

        #region AsyncState
        public static readonly DependencyProperty AsyncStateProperty =
            DependencyProperty.Register("AsyncState", typeof(AsyncType), typeof(AsyncHost),
            new FrameworkPropertyMetadata((AsyncType)AsyncType.Content, new PropertyChangedCallback(OnAsyncStateChanged)));

        public AsyncType AsyncState
        {
            get { return (AsyncType)GetValue(AsyncStateProperty); }
            set { SetValue(AsyncStateProperty, value); }
        }

        private static void OnAsyncStateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AsyncHost)d).OnAsyncStateChanged(e);
        }

        protected virtual void OnAsyncStateChanged(DependencyPropertyChangedEventArgs e)
        {
            foreach (UIElement element in Children)
            {
                if (element.GetValue(AsyncHost.AsyncContentTypeProperty).Equals(e.NewValue))
                    element.Visibility = Visibility.Visible;
                else
                {

                    element.Visibility = ShouldCollapse ? Visibility.Collapsed : Visibility.Hidden;
                }
            }
        }
        #endregion

        #region AsyncContentType
        public static readonly DependencyProperty AsyncContentTypeProperty =
            DependencyProperty.RegisterAttached("AsyncContentType", typeof(AsyncType), typeof(Control));

        public static void SetAsyncContentType(UIElement element, AsyncType value)
        {
            element.SetValue(AsyncContentTypeProperty, value);
        }

        public static AsyncType GetAsyncContentType(UIElement element)
        {
            return (AsyncType)element.GetValue(AsyncContentTypeProperty);
        }
        #endregion
    }

}
