using MEFedMVVM.Services.Contracts;

namespace Cinch.Reloaded.Services.Interfaces
{
    public interface IVSM : IContextAware
    {
        string LastStateExecuted { get; }
        void GoToState(string stateName);
    }
}

