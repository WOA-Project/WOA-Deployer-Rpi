using System;

namespace Deployer.Gui.Core
{
    public interface IViewService
    {
        void Register(string token, Type viewType);
        void Show(string key, object viewModel);
    }
}