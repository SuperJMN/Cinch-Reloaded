using System;
using Cinch.Reloaded.Events.EventArgs;

namespace Cinch.Reloaded.Services.Interfaces
{
    /// <summary>
    /// This interface defines a UI controller which can be used to display dialogs
    /// in either modal or modaless form from a ViewModel.
    /// </summary>
    public interface IUIVisualizerService
    {
        void Register(string key, Type winType);

        bool Unregister(string key);

        bool Show(string key, object state, 
            EventHandler<UICompletedEventArgs> completedProc);

        bool? ShowDialog(string key, object state);
    }
}
