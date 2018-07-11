using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Windows;

using CinchExtended.Events.EventArgs;
using CinchExtended.Services.Interfaces;
using CinchExtended.Workspaces;
using MEFedMVVM.ViewModelLocator;

namespace CinchExtended.Services.Implementation
{
    using CinchExtended.Properties;

    [ExportService(ServiceType.Both, typeof(IUIVisualizerService))]
    [Export(typeof(IExtendedUIVisualizerService))]
    [PartCreationPolicy(CreationPolicy.Shared), UsedImplicitly]
    public class ExtendedWpfUIVisualizerService : WpfUIVisualizerService, IExtendedUIVisualizerService
    {
        public bool Show(string popupKey, object viewModel, string ownerKey, EventHandler<UICompletedEventArgs> completedProc)
        {
            var window = GetFirstWindowWithThisKey(ownerKey);
            return Show(popupKey, window, viewModel, completedProc);
        }

        public bool? ShowDialog(string popupKey, string ownerKey, object viewModel)
        {
            var window = GetFirstWindowWithThisKey(ownerKey);
            return ShowDialog(popupKey, window, viewModel);
        }

        public bool Show(string popupKey, object viewModel, EventHandler<UICompletedEventArgs> completedProc, OwnerOption ownerOption)
        {
            var owner = GetOwner(ownerOption);
            return Show(popupKey, owner, viewModel, completedProc);
        }

        private static Window GetOwner(OwnerOption ownerOption)
        {
            switch (ownerOption)
            {
                case OwnerOption.None:
                    return null;
                case OwnerOption.MainWindow:
                    return Application.Current.MainWindow;
                case OwnerOption.ActiveWindow:
                    return AllWindows.LastOrDefault(window => window.IsActive && window.IsVisible);
                case OwnerOption.MostRecentlyOpen:
                    return AllWindows.LastOrDefault(window => window.IsVisible);
                default:
                    throw new ArgumentException("The OwnerOption is not valid", "ownerOption");
            }
        }

        public bool OwnerExist(OwnerOption ownerToLookFor)
        {
            if (ownerToLookFor == OwnerOption.None)
            {
                return true;
            }
            return GetOwner(ownerToLookFor) == null;
        }

        public bool? ShowDialog(string popupKey, object viewModel, OwnerOption ownerOption)
        {
            var owner = GetOwner(ownerOption);
            return ShowDialog(popupKey, owner, viewModel);
        }

        private static Window GetFirstWindowWithThisKey(string ownerKey)
        {
            var windows = FindPopups(ownerKey);
            var window = windows.First();
            return window;
        }

        private static IEnumerable<Window> FindPopups(string key)
        {
            var allWindows = AllWindows;

            var windowsWithKey = allWindows.Where(window => Equals(GetWindowPopupName(window), key));

            return windowsWithKey;
        }

        private static IEnumerable<Window> AllWindows
        {
            get { return Application.Current.Windows.Cast<Window>(); }
        }

        private static string GetWindowPopupName(Window window)
        {
            var type = window.GetType();
            var attributes = type.GetCustomAttributes(typeof(PopupNameToViewLookupKeyMetadataAttribute), true);
            if (attributes.Any())
            {
                var attr = (PopupNameToViewLookupKeyMetadataAttribute)attributes.First();
                return attr.PopupName;
            }

            return null;
        }
    }
}