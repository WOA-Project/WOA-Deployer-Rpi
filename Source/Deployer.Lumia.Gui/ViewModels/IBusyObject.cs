using System;

namespace Deployer.Lumia.Gui.ViewModels
{
    public interface IBusy
    {
        IObservable<bool> IsBusyObservable { get; }
    }
}