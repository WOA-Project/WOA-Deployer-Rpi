using Deployer.Gui.ViewModels;
using Deployer.Raspberry.Gui.ViewModels;
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
        public StatusViewModel StatusViewModel => container.Locate<StatusViewModel>();
    }
}