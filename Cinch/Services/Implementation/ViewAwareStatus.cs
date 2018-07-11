using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using System.Windows.Threading;
using CinchExtended.Messaging;
using CinchExtended.Services.Interfaces;
using MEFedMVVM.ViewModelLocator;

namespace CinchExtended.Services.Implementation
{
    /// <summary>
    /// View aware service that provides the following
    /// 1. Events for ViewLoaded / ViewUnloaded (WPF and SL)
    /// 2. Events for ViewActivated / ViewDeactivated (WPF Only)
    /// 3. Views current Dispatcher
    /// 4. If the view implements <c>IViewCreationContextProvider</c>
    ///    the current Views Context will also be available to allow
    ///    the ViewModel to obtain some view specific contextual information
    /// </summary>
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ExportService(ServiceType.Both, typeof(IViewAwareStatus))]
    public class ViewAwareStatus : IViewAwareStatus
    {

        #region Data
        WeakReference weakViewInstance;
        #endregion

        #region IViewAwareStatus Members

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

                    ((FrameworkElement)targ).Loaded -= OnViewLoaded;
                    ((FrameworkElement)targ).Unloaded -= OnViewUnloaded;

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

        #endregion
    }
}