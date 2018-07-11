using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Windows;
using Cinch.Reloaded.Events.EventArgs;
using Cinch.Reloaded.Events.WeakEvents;
using Cinch.Reloaded.Services.Interfaces;
using Cinch.Reloaded.ViewModels;
using MEFedMVVM.ViewModelLocator;

namespace Cinch.Reloaded.Services.Implementation
{
 



    /// <summary>
    /// This class implements the IUIVisualizerService for WPF purposes.
    /// If you have attributed up your views using the ViewnameToViewLookupKeyMetadataAttribute
    /// Registration of Views with the IUIVisualizerService service is automatic.
    /// However you can still register views manually, to do this simply put some lines like this in you App.Xaml.cs
    /// ViewModelRepository.Instance.Resolver.Container.GetExport<IUIVisualizerService>().Value.Register("MainWindow", typeof(MainWindow));
    /// </summary>
    [PartCreationPolicy(CreationPolicy.Shared)]
    [ExportService(ServiceType.Both, typeof(IUIVisualizerService))]
    public class WpfUIVisualizerService : IUIVisualizerService
    {
        #region Data
        private readonly Dictionary<string, Type> registeredWindows;
        #endregion

        #region Ctor
        public WpfUIVisualizerService()
        {
            registeredWindows = new Dictionary<string, Type>();
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Registers a collection of entries
        /// </summary>
        /// <param name="startupData"></param>
        public void Register(Dictionary<string, Type> startupData)
        {
            foreach (var entry in startupData)
                Register(entry.Key, entry.Value);
        }

        /// <summary>
        /// Registers a type through a key.
        /// </summary>
        /// <param name="key">Key for the UI dialog</param>
        /// <param name="winType">Type which implements dialog</param>
        public void Register(string key, Type winType)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");
            if (winType == null)
                throw new ArgumentNullException("winType");
            if (!typeof(Window).IsAssignableFrom(winType))
                throw new ArgumentException("winType must be of type Window");

            lock (registeredWindows)
            {
                registeredWindows.Add(key, winType);
            }
        }

        /// <summary>
        /// This unregisters a type and removes it from the mapping
        /// </summary>
        /// <param name="key">Key to remove</param>
        /// <returns>True/False success</returns>
        public bool Unregister(string key)
        {
            if (string.IsNullOrEmpty(key))
                throw new ArgumentNullException("key");

            lock (registeredWindows)
            {
                return registeredWindows.Remove(key);
            }
        }

        /// <summary>
        /// This method displays a modaless dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <param name="setOwner">Set the owner of the window</param>
        /// <param name="completedProc">Callback used when UI closes (may be null)</param>
        /// <returns>True/False if UI is displayed</returns>
        public bool Show(string key, object state,
            EventHandler<UICompletedEventArgs> completedProc)
        {
            Window win = CreateWindow(key, state, null, completedProc, false);
            if (win != null)
            {
                win.Show();
                return true;
            }
            return false;
        }

        /// <summary>
        /// This method displays a modal dialog associated with the given key.
        /// </summary>
        /// <param name="key">Key previously registered with the UI controller.</param>
        /// <param name="state">Object state to associate with the dialog</param>
        /// <returns>True/False if UI is displayed.</returns>
        public bool? ShowDialog(string key, object state)
        {
            Window win = CreateWindow(key, state, null, null, true);
            if (win != null)
                return win.ShowDialog();

            return false;
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// This creates the WPF window from a key.
        /// </summary>
        /// <param name="key">Key</param>
        /// <param name="dataContext">DataContext (state) object</param>
        /// <param name="owner">Owner of the Popup</param>
        /// <param name="completedProc">Callback</param>
        /// <param name="isModal">True if this is a ShowDialog request</param>
        /// <returns>Success code</returns>
        protected Window CreateWindow(string key, object dataContext, Window owner, EventHandler<UICompletedEventArgs> completedProc, bool isModal)
        {
            if (string.IsNullOrEmpty(key))
            {
                throw new ArgumentNullException("key");
            }

            Type type;
            lock (this.registeredWindows)
            {
                if (!this.registeredWindows.TryGetValue(key, out type))
                {
                    return null;
                }
            }
            var window = (Window)Activator.CreateInstance(type);
            if (dataContext is IViewStatusAwareInjectionAware)
            {
                var viewAwareStatusService = ViewModelRepository.Instance.Resolver.Container.GetExport<IViewAwareStatus>().Value;
                viewAwareStatusService.InjectContext(window);
                ((IViewStatusAwareInjectionAware)dataContext).InitialiseViewAwareService(viewAwareStatusService);
            }
            if (dataContext is IViewStatusAwareWindowInjectionAware)
            {
                var viewAwareStatusServiceWindow = ViewModelRepository.Instance.Resolver.Container.GetExport<IViewAwareStatusWindow>().Value;
                viewAwareStatusServiceWindow.InjectContext(window);
                ((IViewStatusAwareWindowInjectionAware)dataContext).InitialiseViewAwareWindowService(viewAwareStatusServiceWindow);
            }
            window.DataContext = dataContext;

            window.Owner = owner;

            if (dataContext != null)
            {
                ViewModelBase bvm = dataContext as ViewModelBase;
                if (bvm != null)
                {
                    if (isModal)
                        bvm.CloseRequest += EventHandlerUtils.MakeWeak((s, e) =>
                        {
                            try
                            {
                                window.DialogResult = e.Result;
                            }
                            catch (InvalidOperationException exception_0)
                            {
                                window.Close();
                            }
                        }, (UnregisterCallback<CloseRequestEventArgs>)(eh => bvm.CloseRequest -= eh));
                    else
                        bvm.CloseRequest += EventHandlerUtils.MakeWeak((s, e) => window.Close(), (UnregisterCallback<CloseRequestEventArgs>)(eh => bvm.CloseRequest -= eh));
                    bvm.ActivateRequest += EventHandlerUtils.MakeWeak((s, e) => window.Activate(), (UnregisterCallback<EventArgs>)(eh => bvm.ActivateRequest -= eh));
                }
            }

            window.Closed += (EventHandler)((s, e) =>
            {
                window.DataContext = null;
                if (completedProc == null)
                {
                    return;
                }
                completedProc(this, new UICompletedEventArgs()
                {
                    State = dataContext,
                    Result = isModal ? window.DialogResult : new bool?()
                });
            });
            return window;
        }
        #endregion

        protected bool Show(string popupKey, Window owner, object viewModel, EventHandler<UICompletedEventArgs> completedProc)
        {
            var window = this.CreateWindow(popupKey, viewModel, owner, completedProc, false);
            if (window == null)
            {
                return false;
            }
            window.Show();
            return true;
        }

        protected bool? ShowDialog(string popupKey, Window owner, object viewModel)
        {
            if (owner !=null && !owner.IsVisible)
            {
                throw new InvalidOperationException("Cannot set the owner because it has not been shown yet!");
            }

            var window = CreateWindow(popupKey, viewModel, owner, null, true);
            if (window != null)
            {
                return window.ShowDialog();
            }
            return false;
        }
    }
}
