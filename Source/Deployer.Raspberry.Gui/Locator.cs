using Deployer.Lumia.Gui.ViewModels;
using Deployer.Raspberry.Gui.ViewModels;
using Deployer.UI.ViewModels;
using Grace.DependencyInjection;

namespace Deployer.Raspberry.Gui
{
    public class Locator
    {
        private readonly DependencyInjectionContainer container;

        public Locator()
        {
            container = CompositionRoot.CreateContainer();
        }

        public MainViewModel MainViewModel => container.Locate<MainViewModel>();

        public WimPickViewModel WimPickViewModel => container.Locate<WimPickViewModel>();

        public DeploymentViewModel DeploymentViewModel => container.Locate<DeploymentViewModel>();

        public AdvancedViewModel AdvancedViewModel => container.Locate<AdvancedViewModel>();

        public LogViewModel LogViewModel => container.Locate<LogViewModel>();

        public OngoingOperationViewModel OngoingOperationViewModel => container.Locate<OngoingOperationViewModel>();
    }
}