using Cinch.Reloaded.Services.Interfaces;

namespace Cinch.Reloaded.ViewModels
{
    public interface IViewStatusAwareInjectionAware
    {
        void InitialiseViewAwareService(IViewAwareStatus viewAwareStatusService);
    }
}
