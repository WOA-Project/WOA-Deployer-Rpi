using System;

namespace Deployer.Raspberry.Gui.ViewModels
{
    public interface IBusy
    {
        IObservable<bool> IsBusyObservable { get; }
    }
}