using System;

namespace Deployer.Gui.Core
{
    public interface IBusy
    {
        IObservable<bool> IsBusyObservable { get; }
    }
}