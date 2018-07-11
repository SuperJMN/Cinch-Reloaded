namespace Cinch.Reloaded.ViewModels
{
    public interface ICinchDisposable
    {
        // Summary:
        //     Performs Cinch related resource cleaning, such as unhooking
        //     Mediator message registration, etc etc
        void Dispose();
    }
}
