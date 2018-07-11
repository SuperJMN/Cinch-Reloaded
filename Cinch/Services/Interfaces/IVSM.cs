using MEFedMVVM.Services.Contracts;

namespace CinchExtended.Services.Interfaces
{
    public interface IVSM : IContextAware
    {
        string LastStateExecuted { get; }
        void GoToState(string stateName);
    }
}

