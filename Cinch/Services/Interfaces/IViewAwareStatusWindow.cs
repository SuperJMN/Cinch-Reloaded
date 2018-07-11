using System;
using System.ComponentModel;
using System.Windows.Threading;
using MEFedMVVM.Services.Contracts;

namespace CinchExtended.Services.Interfaces
{
    public interface IViewAwareStatusWindow : IContextAware
    {
        event Action ViewLoaded;
        event Action ViewUnloaded;
        event Action ViewActivated;
        event Action ViewDeactivated;

        event Action ViewWindowClosed;
        event Action ViewWindowContentRendered;
        event Action ViewWindowLocationChanged;
        event Action ViewWindowStateChanged;
        event EventHandler<CancelEventArgs> ViewWindowClosing;


        Dispatcher ViewsDispatcher { get; }
        Object View { get; }
    }
}
