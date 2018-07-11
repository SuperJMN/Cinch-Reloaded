using CinchExtended.Services.Interfaces;

namespace CinchExtended.ViewModels
{
    public interface IViewStatusAwareWindowInjectionAware
    {
        void InitialiseViewAwareWindowService(IViewAwareStatusWindow viewAwareStatusServiceWindow);
    }
}
