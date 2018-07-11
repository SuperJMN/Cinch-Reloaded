using CinchExtended.Services.Interfaces;

namespace CinchExtended.ViewModels
{
    public interface IViewStatusAwareInjectionAware
    {
        void InitialiseViewAwareService(IViewAwareStatus viewAwareStatusService);
    }
}
