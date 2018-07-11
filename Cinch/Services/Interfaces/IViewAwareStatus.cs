using System;
using System.Windows.Threading;
using MEFedMVVM.Services.Contracts;

namespace CinchExtended.Services.Interfaces
{
    public interface IViewAwareStatus : IContextAware
    {
        event Action ViewLoaded;
        event Action ViewUnloaded;

#if !SILVERLIGHT

        event Action ViewActivated;
        event Action ViewDeactivated;
#else
        void PerformCleanUp();
#endif

        Dispatcher ViewsDispatcher { get; }
        Object View { get; }
    }
}
