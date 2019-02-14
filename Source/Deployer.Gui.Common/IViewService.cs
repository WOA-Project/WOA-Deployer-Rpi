using System;

namespace Deployer.Gui.Common
{
    public interface IViewService
    {
        void Register(string token, Type viewType);
        void Show(string key, object viewModel);
    }
}