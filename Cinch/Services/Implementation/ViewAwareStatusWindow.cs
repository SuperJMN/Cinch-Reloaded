using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Threading;
using CinchExtended.Events.WeakEvents;
using CinchExtended.Messaging;
using CinchExtended.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;

namespace CinchExtended.Services.Implementation
{
    /// <summary>
    /// View aware service that provides the following Events. Where we are specifically
    /// targetting a Window type. As such this is only available for WPF
    ///    ViewLoaded / ViewUnloaded
    ///    ViewActivated / ViewDeactivated
    ///    ViewWindowClosed / ViewWindowContentRendered / 
    ///    ViewWindowLocationChanged / ViewWindowStateChanged
    /// 3. Views current Dispatcher
    /// 4. If the view implements <c>IViewCreationContextProvider</c>
    ///    the current Views Context will also be available to allow
    ///    the ViewModel to obtain some view specific contextual information
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportService(ServiceType.Both, typeof(IViewAwareStatusWindow))]
    public class ViewAwareStatusWindow : IViewAwareStatusWindow
    {

        #region Data
        WeakReference weakViewInstance;
        #endregion

        #region IViewAwareStatusWindow Members

        readonly IList<WeakAction> loadedHandlers = new List<WeakAction>();
        public event Action ViewLoaded
        {
            add
            {
                loadedHandlers.Add(new WeakAction(value.Target, typeof(Action), value.Method));
            }
            remove
            {

            }
        }


        readonly IList<WeakAction> unloadedHandlers = new List<WeakAction>();
        public event Action ViewUnloaded
        {
            add
            {
                unloadedHandlers.Add(new WeakAction(value.Target, typeof(Action), value.Method));
            }
            remove
            {

            }
        }


        readonly IList<WeakAction> activatedHandlers = new List<WeakAction>();
        public event Action ViewActivated
        {
            add
            {
                activatedHandlers.Add(new WeakAction(value.Target, typeof(Action), value.Method));
            }
            remove
            {

            }
        }


        readonly IList<WeakAction> deactivatedHandlers = new List<WeakAction>();
        public event Action ViewDeactivated
        {
            add
            {
                deactivatedHandlers.Add(new WeakAction(value.Target, typeof(Action), value.Method));
            }
            remove
            {

            }
        }


        readonly IList<WeakAction> closedHandlers = new List<WeakAction>();
        public event Action ViewWindowClosed
        {
            add
            {
                closedHandlers.Add(new WeakAction(value.Target, typeof(Action), value.Method));
            }
            remove
            {

            }
        }

        private readonly WeakEvent<EventHandler<CancelEventArgs>> viewWindowClosingEvent = new WeakEvent<EventHandler<CancelEventArgs>>();
        public event EventHandler<CancelEventArgs> ViewWindowClosing
        {
            add
            {
                viewWindowClosingEvent.Add(value);
            }
            remove
            {
                viewWindowClosingEvent.Remove(value);
            }
        }

        readonly IList<WeakAction> contentRenderedHandlers = new List<WeakAction>();
        public event Action ViewWindowContentRendered
        {
            add
            {
                contentRenderedHandlers.Add(new WeakAction(value.Target, typeof(Action), value.Method));
            }
            remove
            {

            }
        }

        readonly IList<WeakAction> locationChangedHandlers = new List<WeakAction>();
        public event Action ViewWindowLocationChanged
        {
            add
            {
                locationChangedHandlers.Add(new WeakAction(value.Target, typeof(Action), value.Method));
            }
            remove
            {

            }
        }

        readonly IList<WeakAction> stateChangedHandlers = new List<WeakAction>();
        public event Action ViewWindowStateChanged
        {
            add
            {
                stateChangedHandlers.Add(new WeakAction(value.Target, typeof(Action), value.Method));
            }
            remove
            {

            }
        }


        public Dispatcher ViewsDispatcher { get; private set; }

        public Object View
        {
            get
            {
                return (Object)weakViewInstance.Target;
            }
        }
        #endregion

        #region IContextAware Members

        public void InjectContext(object view)
        {
            if (this.weakViewInstance != null)
            {
                if (this.weakViewInstance.Target == view)
                    return;
            }

            // unregister before hooking new events
            if (this.weakViewInstance != null && this.weakViewInstance.Target != null)
            {

                object targ = this.weakViewInstance.Target;

                if (targ != null)
                {

                    ((FrameworkElement)this.weakViewInstance.Target).Loaded -= OnViewLoaded;
                    ((FrameworkElement)this.weakViewInstance.Target).Unloaded -= OnViewUnloaded;
                    ((Window)this.weakViewInstance.Target).Closed -= OnViewWindowClosed;
                    ((Window)this.weakViewInstance.Target).Closing -= OnViewWindowClosing;
                    ((Window)this.weakViewInstance.Target).ContentRendered -= OnViewWindowContentRendered;
                    ((Window)this.weakViewInstance.Target).LocationChanged -= OnViewWindowLocationChanged;
                    ((Window)this.weakViewInstance.Target).StateChanged -= OnViewWindowStateChanged;

                    Window w = targ as Window;
                    if (w != null)
                    {
                        w.Activated -= OnViewActivated;
                        w.Deactivated -= OnViewDeactivated;
                    }
                }

            }

            var x = view as FrameworkElement;

            if (x != null)
            {
                x.Loaded += OnViewLoaded;
                x.Unloaded += OnViewUnloaded;

                Window w = x as Window;
                if (w != null)
                {
                    w.Activated += OnViewActivated;
                    w.Deactivated += OnViewDeactivated;
                    w.Closed += OnViewWindowClosed;
                    w.Closing += OnViewWindowClosing;
                    w.ContentRendered += OnViewWindowContentRendered;
                    w.LocationChanged += OnViewWindowLocationChanged;
                    w.StateChanged += OnViewWindowStateChanged;
                }

                //get the Views Dispatcher
                this.ViewsDispatcher = x.Dispatcher;
                weakViewInstance = new WeakReference(x);

            }
        }
        #endregion

        #region Private Helpers

        private void OnViewLoaded(object sender, RoutedEventArgs e)
        {
            foreach (var loadedHandler in loadedHandlers)
            {
                loadedHandler.GetMethod().DynamicInvoke();
            }
        }

        private void OnViewUnloaded(object sender, RoutedEventArgs e)
        {
            foreach (var unloadedHandler in unloadedHandlers)
            {
                unloadedHandler.GetMethod().DynamicInvoke();
            }
        }


        private void OnViewActivated(object sender, EventArgs e)
        {
            foreach (var activatedHandler in activatedHandlers)
            {
                activatedHandler.GetMethod().DynamicInvoke();
            }

        }

        private void OnViewDeactivated(object sender, EventArgs e)
        {
            foreach (var deactivatedHandler in deactivatedHandlers)
            {
                deactivatedHandler.GetMethod().DynamicInvoke();
            }
        }


        private void OnViewWindowClosed(object sender, EventArgs e)
        {
            foreach (var closedHandler in closedHandlers)
            {
                closedHandler.GetMethod().DynamicInvoke();
            }
        }

        private void OnViewWindowClosing(object sender, CancelEventArgs e)
        {
            viewWindowClosingEvent.Raise(this, e);
        }

        private void OnViewWindowContentRendered(object sender, EventArgs e)
        {
            foreach (var contentRenderedHandler in contentRenderedHandlers)
            {
                contentRenderedHandler.GetMethod().DynamicInvoke();
            }
        }

        private void OnViewWindowLocationChanged(object sender, EventArgs e)
        {
            foreach (var locationChangedHandler in locationChangedHandlers)
            {
                locationChangedHandler.GetMethod().DynamicInvoke();
            }
        }

        private void OnViewWindowStateChanged(object sender, EventArgs e)
        {
            foreach (var stateChangedHandler in stateChangedHandlers)
            {
                stateChangedHandler.GetMethod().DynamicInvoke();
            }
        }
        #endregion
    }
}