using Cinch.Reloaded.Services.Interfaces;

namespace Cinch.Reloaded.ViewModels
{
    public interface IViewStatusAwareWindowInjectionAware
    {
        void InitialiseViewAwareWindowService(IViewAwareStatusWindow viewAwareStatusServiceWindow);
    }
}
