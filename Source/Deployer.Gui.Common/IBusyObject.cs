using System;

namespace Deployer.Gui.Common
{
    public interface IBusy
    {
        IObservable<bool> IsBusyObservable { get; }
    }
}