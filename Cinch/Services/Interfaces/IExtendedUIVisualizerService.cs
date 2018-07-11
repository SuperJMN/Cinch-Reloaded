using System;
using CinchExtended.Events.EventArgs;
using CinchExtended.Services.Implementation;

namespace CinchExtended.Services.Interfaces
{
    public interface IExtendedUIVisualizerService : IUIVisualizerService
    {        
        bool Show(string popupKey, object viewModel, EventHandler<UICompletedEventArgs> completedProc, OwnerOption ownerOption);
        bool Show(string popupKey, object viewModel, string ownerKey, EventHandler<UICompletedEventArgs> completedProc);

        bool? ShowDialog(string popupKey, string ownerKey, object viewModel);
        bool? ShowDialog(string popupKey, object viewModel, OwnerOption ownerOption);

        bool OwnerExist(OwnerOption ownerToLookFor);
    }
}